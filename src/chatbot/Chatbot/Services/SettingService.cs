namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Params.Db;
    using Fanex.Caching;
    using Fanex.Data.Repository;

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

        Task<Result> Unsubscribe(int userId, int messengerType);
    }

    public class SettingServiceImpl : SettingService
    {
        private const int MaxAddActionTimes = 10;
        private const int AddAccountActionId = 100;
        private const int WrongVerificationCodeActionId = 101;
        private const string functionNameByLanguageCacheKey = "SettingServiceImpl.FunctionNameByLanguageCacheKey.{0}";
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
            var client = messengerFactory.CreateMessengerClient(messenger.Type);

            var command = new InsertVerificationCodeCommand(userId, messenger.Type, verificationCode);
            var returnCode = await dynamicRepository.ExecuteAsync(command);

            if (returnCode != 0)
            {
                throw new InvalidOperationException($"Failed insert Verification Code with code {returnCode}");
            }

            var result = await client.PushAsync(messenger.Account, verificationCode);

            if (result.IsNotOk)
            {
                throw new InvalidOperationException(result.Message);
            }

            return new Result(result.Code, result.Message);
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

            var functions = await dynamicRepository.FetchAsync<Function>(new GetFunctionsCriteria(userId));

            foreach (var function in functions)
            {
                var functionLanguages = await cacheService.GetOrSetAsync(
                        string.Format(functionNameByLanguageCacheKey, function.Id),
                        () => GetFunctionLanguage(function.Id).Result,
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)));

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

            var command = new UpdateFunctionsCommand(userId, functions);
            var code = await dynamicRepository.ExecuteAsync(command);

            return new Result(code);
        }

        public async Task<Result> UpdateLanguage(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            var command = new UpdateLanguageCommand(userId, language);
            var code = await dynamicRepository.ExecuteAsync(command);

            return new Result(code);
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

            await UpdateFailedActionCount(userId, AddAccountActionId, messenger.Type);

            if(await IsUserAddAccountOverMaxTimeInDay(userId, messenger.Type))
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

            await GenerateVerificationCode(userId, messenger);

            return AddingUserResult.CreateSuccessfulResult(userId);
        }

        private async Task<bool> IsMessengerAccountBelongToAnotherUser(int userId, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messenger), messenger);

            var criteria = new GetMessengerCriteria(messenger.Account, messenger.Type);
            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(criteria);

            if (messengerDto != null
                && messengerDto.UserId != userId)
            {
                return true;
            }

            return false;
        }

        private async Task<int> InsertUser(int custId, int subAccountId, string language)
        {
            var command = new InsertUserCommand(custId, subAccountId, language);

            await dynamicRepository.ExecuteAsync(command);

            return command.UserId;
        }

        private async Task<bool> IsUserAddAccountOverMaxTimeInDay(int userId, int messengerType)
        {
            var numOfAddTime = (await GetUserAction(userId, messengerType, AddAccountActionId)).ActionCount;

            return numOfAddTime > MaxAddActionTimes;
        }

        private async Task<UserActionDto> GetUserAction(int userId, int messengerType, int actionId)
        {
            var criteria = new GetFailedActionCriteria(userId, actionId, messengerType);
            var userAction = await dynamicRepository.GetAsync<UserActionDto>(criteria);

            return userAction;
        }

        private async Task<bool> IsMessengerAccountNotExisted(Messenger messenger)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var profileResult = await client.GetProfileAsync(messenger.Account);

            return !profileResult.Item2;
        }

        public async Task<Result> Unsubscribe(int userId, int messengerType)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            var command = new DisabledMessengerCommand(userId, messengerType);
            var code = await dynamicRepository.ExecuteAsync(command);

            return new Result(code);
        }

        public async Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messenger), messenger);

            var criteria = new GetMessengerByUserCriteria(userId, messenger.Type);
            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(criteria);

            if (messengerDto == null
                || messengerDto.Account != messenger.Account)
            {
                return VerificationCodeResult.CreateWrongVerificationCodeResult();
            }

            if (messengerDto.GeneratedVerificationCodeTime.AddMinutes(5) > getCurrentDateTimeFunc())
            {
                return VerificationCodeResult.CreateVerificationCodeExpiredResult();
            }

            if (messengerDto.VerificationCode != verificationCode)
            {
                await UpdateFailedActionCount(userId, WrongVerificationCodeActionId, messenger.Type);
            }

            return VerificationCodeResult.CreateVerificationSuccessfulResult();
        }

        private async Task<Result> UpdateFailedActionCount(int userId, int actionId, int messengerType)
        {
            var command = new UpdateFailActionCommand(userId, actionId, messengerType);
            var code = await dynamicRepository.ExecuteAsync(command);

            return new Result(code);
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