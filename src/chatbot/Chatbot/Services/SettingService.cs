namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Params.Db.Criteria;
    using Chatbot.Params.Db.Commands;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MessengerClient = Messenger.Client;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int userId, string language);

        Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task<Result> UpdateLanguage(int userId, string language);

        Task<Result> GenerateVerificationCode(int userId, Messenger messenger);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, Messenger messenger);

        Task<IEnumerable<Function>> GetAllFunctions();

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<Result> UpdateMessengerStatus(int userId, int messengerType, string messengerUsername, bool enabled);
    }

    public class SettingServiceImpl : SettingService
    {
        private const int MaxAddActionTimes = 10;
        private const int AddAccountActionId = 100;
        private const int WrongVerificationCodeActionId = 101;
        private const int VerificationCodeExpiredTimeInMinutes = 5;
        private const string functionNameByLanguageCacheKey = "SettingServiceImpl.FunctionNameByLanguageCacheKey.{0}";
        private readonly CacheItemOptions cacheItemOptions = new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0));
        private readonly IDynamicRepository dynamicRepository;
        private readonly MessengerFactory messengerFactory;
        private readonly Func<string> randomNumberFunc;
        private readonly Func<DateTime> getCurrentDateTimeFunc;
        private readonly ICacheService cacheService;

        public SettingServiceImpl(
            IDynamicRepository dynamicRepository,
            MessengerFactory messengerFactory,
            ICacheService cacheService,
            Func<string> randomNumberFunc = null,
            Func<DateTime> getCurrentDateTimeFunc = null)
        {
            this.dynamicRepository = dynamicRepository;
            this.messengerFactory = messengerFactory;
            this.randomNumberFunc = randomNumberFunc ?? RandomSixDigitsNumber;
            this.cacheService = cacheService ?? new CacheService();
            this.getCurrentDateTimeFunc = getCurrentDateTimeFunc ?? GetCurrentDateTime;
        }

        public async Task<Result> GenerateVerificationCode(int userId, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messenger), messenger);

            var verificationCode = RandomSixDigitsNumber();

            await InsertMessengerWithVerificationCode(userId, messenger, verificationCode);
            await SendMessage(messenger, $"Your Letstalk validation code is {verificationCode}. The validation code will be expired after 5 minutes.");

            return Result.CreateSuccessfulResult();
        }

        private async Task<MessengerClient.Models.Result> SendMessage(
            Messenger messenger, 
            string message)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var result = await client.PushAsync(messenger.Username, message);

            if (result.IsNotOk)
            {
                throw new InvalidOperationException(result.Message);
            }

            return result;
        }

        private async Task<bool> InsertMessengerWithVerificationCode(
            int userId, 
            Messenger messenger, 
            string verificationCode)
        {
            var command = new InsertMessengerCommand(userId, messenger.Type, messenger.Username, verificationCode);

            await dynamicRepository.ExecuteAsync(command);

            if(command.IsCommandRunFailed)
            {
                throw new InvalidOperationException($"Insert Messenger With Verification Code failed with code {command.Code}");
            }

            return command.IsCommandRunSuccessfully;
        }

        private string RandomSixDigitsNumber()
            => new Random().Next(0, 999999).ToString("D6");

        private DateTime GetCurrentDateTime()
            => DateTime.Now;

        public async Task<IEnumerable<Function>> GetAllFunctions()
            => await dynamicRepository.FetchAsync<Function>(new GetAllFunctionsCriteria());

        public async Task<User> GetUser(int custId, int subAccountId, string language)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(language), language);

            var user = await dynamicRepository.GetAsync<User>(new GetUserCriteria(custId, subAccountId));

            if (user == null)
            {
                return user;
            }

            user.MessengerAccounts = await dynamicRepository.FetchAsync<Messenger>(new GetMessengerByUserCriteria(user.Id));
            user.Functions = await GetFunctions(user.Id, language);

            return user;
        }

        public async Task<IEnumerable<Function>> GetFunctions(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            var functions = await dynamicRepository.FetchAsync<Function>(new GetUserFunctionsCriteria(userId));

            foreach (var function in functions)
            {
                var functionLanguages = await cacheService.GetOrSetAsync(
                        string.Format(functionNameByLanguageCacheKey, function.Id),
                        () => GetFunctionLanguage(function.Id).Result,
                        cacheItemOptions);

                function.Name = functionLanguages
                    .FirstOrDefault(languageDto => languageDto.LanguageCode == language)
                    ?.FunctionName;
            }

            return functions;
        }

        private async Task<IEnumerable<FunctionLanguageDto>> GetFunctionLanguage(int functionId)
            => await dynamicRepository.FetchAsync<FunctionLanguageDto>(new GetFunctionNameCriteria(functionId));

        public async Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(functions), functions);

            var command = new UpdateUserFunctionsCommand(userId, functions);

            await dynamicRepository.ExecuteAsync(command);

            if(command.IsCommandRunFailed)
            {
                throw new InvalidOperationException($"Update Functions failed with code {command.Code}");
            }

            return Result.CreateSuccessfulResult();
        }

        public async Task<Result> UpdateLanguage(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            var command = new UpdateUserLanguageCommand(userId, language);

            await dynamicRepository.ExecuteAsync(command);

            if (command.IsCommandRunFailed)
            {
                throw new InvalidOperationException($"Update Language failed with code {command.Code}");
            }

            return Result.CreateSuccessfulResult();
        }

        /// <summary>
        /// Need more refactor here
        /// </summary>
        public async Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, Messenger messenger)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(language), language);
            ValidateInputParam(nameof(messenger), messenger);

            var userId = await InsertUser(custId, subAccountId, language);

            await UpdateUserActionCount(
                userId, 
                AddAccountActionId, 
                messenger.Type,
                messenger.Username);

            var result = await VerifyMessengerAccount(userId, messenger);
            if(result.IsNotOk)
            {
                return result;
            }

            await GenerateVerificationCode(userId, messenger);

            return AddingUserResult.CreateSuccessfulResult(userId);
        }

        private async Task<AddingUserResult> VerifyMessengerAccount(int userId, Messenger messenger)
        {
            if (await IsUserAddAccountOverMaxTimeInDay(userId, messenger.Type, messenger.Username))
            {
                return AddingUserResult.CreateIsOverMaxAddAccountTimesResult();
            }

            if (await IsMessengerAccountNotExisted(messenger))
            {
                return AddingUserResult.CreateAccountDoesNotExistResult();
            }

            if (await IsMessengerAccountBelongToAnotherUser(userId, messenger))
            {
                return AddingUserResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();
            }

            return AddingUserResult.CreateSuccessfulResult();
        }

        private async Task<bool> IsMessengerAccountBelongToAnotherUser(int userId, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messenger), messenger);

            var criteria = new GetMessengerCriteria(messenger.Username, messenger.Type);
            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(criteria);

            return messengerDto != null
                && messengerDto.UserId != userId;
        }

        private async Task<int> InsertUser(int custId, int subAccountId, string language)
        {
            var command = new InsertUserCommand(custId, subAccountId, language);

            await dynamicRepository.ExecuteAsync(command);

            if (command.IsCommandRunFailed)
            {
                throw new InvalidOperationException($"Insert User failed with code {command.Code}");
            }

            return command.UserId;
        }

        private async Task<bool> IsUserAddAccountOverMaxTimeInDay(
            int userId, 
            int messengerType,
            string messengerUsername)
        {
            var addUserLogActions = await GetUserAction(userId, messengerType, messengerUsername, AddAccountActionId);
            var numOfAddTime = addUserLogActions.Sum(action => action.CountTimes);

            return numOfAddTime > MaxAddActionTimes;
        }

        private async Task<IEnumerable<UserActionDto>> GetUserAction(
            int userId, 
            int messengerType, 
            string messengerUsername,
            int actionId)
        {
            var criteria = new GetUserActionsCriteria(userId, actionId, messengerType, messengerUsername);

            var userAction = await dynamicRepository.FetchAsync<UserActionDto>(criteria);

            return userAction;
        }

        private async Task<bool> IsMessengerAccountNotExisted(Messenger messenger)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);

            var profileResult = await client.GetProfileAsync(messenger.Username);

            return !profileResult.Item2;
        }

        public async Task<Result> UpdateMessengerStatus(
            int userId, 
            int messengerType, 
            string messengerUsername, 
            bool enabled)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            var command = new UpdateMessengerStatusCommand(userId, messengerType, messengerUsername, enabled);

            await dynamicRepository.ExecuteAsync(command);

            if (command.IsCommandRunFailed)
            {
                throw new InvalidOperationException($"Update Messenger status failed with code {command.Code}");
            }

            return Result.CreateSuccessfulResult();
        }

        public async Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messenger), messenger);

            var verificationResult = await CheckVerificationCode(userId, verificationCode, messenger);

            if(verificationResult.IsNotOk)
            {
                return verificationResult;
            }

            var result = await UpdateMessengerStatus(userId, messenger.Type, messenger.Username, true);

            return new VerificationCodeResult(result.Code, result.Message);
        }

        private async Task<VerificationCodeResult> CheckVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            var criteria = new GetMessengerByUserCriteria(userId, messenger.Type);
            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(criteria);
            var result = VerificationCodeResult.CreateVerificationSuccessfulResult();

            if (messengerDto == null
                || messengerDto.Username != messenger.Username)
            {
                result = VerificationCodeResult.CreateWrongVerificationCodeResult();
            }
            else if (messengerDto.GeneratedVerificationCodeTime.AddMinutes(VerificationCodeExpiredTimeInMinutes) < getCurrentDateTimeFunc())
            {
                result = VerificationCodeResult.CreateVerificationCodeExpiredResult();
            }
            else if (messengerDto.VerificationCode != verificationCode)
            {
                result = VerificationCodeResult.CreateWrongVerificationCodeResult();
            }

            if(result.IsNotOk)
            {
                await UpdateUserActionCount(
                        userId,
                        WrongVerificationCodeActionId,
                        messenger.Type,
                        messenger.Username);

                var failedAction = (await GetUserAction(userId, messenger.Type, messenger.Username, WrongVerificationCodeActionId)).FirstOrDefault();

                result = new VerificationCodeResult(
                    result.Code,
                    result.Message,
                    failedAction != null
                        ? failedAction.CountTimes
                        : 0);
            }

            return result;
        }

        private async Task<Result> UpdateUserActionCount(
            int userId, 
            int actionId, 
            int messengerType,
            string messengerUsername)
        {
            var command = new InsertUserActionCommand(userId, actionId, messengerType, messengerUsername);

            await dynamicRepository.ExecuteAsync(command);

            if (command.IsCommandRunFailed)
            {
                throw new InvalidOperationException($"Update Language failed with code {command.Code}");
            }

            return Result.CreateSuccessfulResult();
        }

        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateInputParam(string paramName, int paramValue)
        {
            if (paramValue <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        private static void ValidateInputParam<T>(string paramName, IEnumerable<T> paramValue)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!paramValue.Any())
            {
                throw new ArgumentOutOfRangeException($"{paramName} does not contain any item");
            }
        }
    }
}