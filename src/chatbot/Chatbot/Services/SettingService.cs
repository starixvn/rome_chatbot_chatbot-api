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

        Task<Result> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, Messenger messenger);

        Task<IEnumerable<Function>> GetAllFunctions();

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<Result> Unsubscribe(int userId, int messengerType);
    }

    public class SettingServiceImpl : SettingService
    {
        private const int NotExistAccountActionId = 100;
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

            var returnCode = await dynamicRepository.ExecuteAsync(new InsertVerificationCodeCommand(userId, messenger.Type, verificationCode));

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
                        () => dynamicRepository.FetchAsync<FunctionLanguageDto>(new GetFunctionNameCriteria(function.Id)).Result,
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)));

                function.Name = functionLanguages
                    .FirstOrDefault(languageDto => languageDto.LanguageCode == language)
                    ?.FunctionName;
            }

            return functions;
        }

        public async Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(functions), functions);

            var code = await dynamicRepository.ExecuteAsync(new UpdateFunctionsCommand(userId, functions));

            return new Result(code);
        }

        public async Task<Result> UpdateLanguage(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            var code = await dynamicRepository.ExecuteAsync(new UpdateLanguageCommand(userId, language));

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
            var isExisted = await CheckMessengerAccountExisted(messenger);

            if (!isExisted)
            {
                await UpdateFailedActionCount(userId, NotExistAccountActionId, messenger.Type);

                return AddingUserResult.CreateAccountDoesNotExistResult();
            }

            if (await IsMessengerAccountBelongToAnotherUser(userId, messenger))
            {
                return AddingUserResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();
            }

            // Need refactor code here
            await GenerateVerificationCode(userId, messenger);

            return AddingUserResult.CreateSuccessfulResult(userId);
        }

        private async Task<bool> IsMessengerAccountBelongToAnotherUser(int userId, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messenger), messenger);

            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(new GetMessengerCriteria(messenger.Account, messenger.Type));

            if (messengerDto != null
                && messengerDto.UserId != userId)
            {
                return true;
            }

            return false;
        }

        private async Task<int> InsertUser(int custId, int subAccountId, string language)
        {
            var insertCommand = new InsertUserCommand(custId, subAccountId, language);

            await dynamicRepository.ExecuteAsync(insertCommand);

            return insertCommand.UserId;
        }

        private async Task<bool> CheckMessengerAccountExisted(Messenger messenger)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var profileResult = await client.GetProfileAsync(messenger.Account);

            return profileResult.Item2;
        }

        public async Task<Result> Unsubscribe(int userId, int messengerType)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            var code = await dynamicRepository.ExecuteAsync(new DisabledMessengerCommand(userId, messengerType));

            return new Result(code);
        }

        public async Task<Result> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messenger), messenger);

            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(new GetMessengerByUserCriteria(userId, messenger.Type));

            if (messengerDto == null
                || messengerDto.Account != messenger.Account)
            {
                return Result.CreateWrongVerificationCodeResult();
            }

            if (messengerDto.GeneratedVerificationCodeTime.AddMinutes(5) > getCurrentDateTimeFunc())
            {
                return Result.CreateVerificationCodeExpiredResult();
            }

            if (messengerDto.VerificationCode != verificationCode)
            {
                await UpdateFailedActionCount(userId, WrongVerificationCodeActionId, messenger.Type);
            }

            return Result.CreateSuccessfulResult();
        }

        private async Task<Result> UpdateFailedActionCount(int userId, int actionId, int messengerType)
        {
            var code = await dynamicRepository.ExecuteAsync(new UpdateFailActionCommand(userId, actionId, messengerType));

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