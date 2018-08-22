namespace Chatbot.Modules.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int userId, string language = "");

        Task UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task UpdateLanguage(int userId, string language);

        Task GenerateVerificationCode(int userId, string custName, Messenger messenger);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string custName, string language, Messenger messenger);

        Task<IEnumerable<Function>> GetAllFunctions();

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<User> GetUser(int userId);

        Task UpdateMessengerStatus(int userId, int messengerType, string messengerUsername, bool enabled);

        Task<MessengerDto> GetMessenger(int messengerType, string messengerUsername);

        Task<IEnumerable<User>> GetUsersByFunction(int functionId, int environment);

        Task<IEnumerable<Messenger>> GetMessengers(int userId, bool isEnabledOnly = false);
    }

    public class SettingServiceImpl : SettingService
    {
        private const string DefaultLanguage = "en-US";
        private const string functionNameByLanguageCacheKey = "SettingServiceImpl.FunctionNameByLanguageCacheKey.{0}";
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<string> randomNumberFunc;
        private readonly Func<DateTime> getCurrentDateTimeFunc;
        private readonly ICacheService cacheService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;

        public SettingServiceImpl(
            IDynamicRepository dynamicRepository,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            ICacheService cacheService,
            IMessageLogger messageLogger,
            IChatbotConfigs chatbotConfigs,
            Func<string> randomNumberFunc = null,
            Func<DateTime> getCurrentDateTimeFunc = null)
        {
            this.dynamicRepository = dynamicRepository;
            this.randomNumberFunc = randomNumberFunc ?? RandomSixDigitsNumber;
            this.cacheService = cacheService ?? new CacheService();
            this.getCurrentDateTimeFunc = getCurrentDateTimeFunc ?? GetCurrentDateTime;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.messageLogger = messageLogger;
            this.chatbotConfigs = chatbotConfigs;
        }

        public async Task GenerateVerificationCode(int userId, string custName, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messenger), messenger);
            ValidateInputParam("Messenger Username", messenger.Username);
            ValidateInputParam("Messenger Type", messenger.Type);

            var verificationCode = RandomSixDigitsNumber();
            var user = await GetUser(userId);

            await Task.WhenAll(
                InsertMessengerWithVerificationCode(userId, messenger, verificationCode),
                SendVerificationCode(user, custName, messenger, verificationCode));
        }

        private async Task SendVerificationCode(User user, string custName, Messenger messenger, string verificationCode)
        {
            var message = messageBuilder.BuildVerificationCodeMessage(
                custName,
                user.Language,
                messenger.Type,
                verificationCode,
                chatbotConfigs.VerificationCodeExpiredTimeInMinutes);

            await messageSender.SendMessage(user.Id, messenger, message);
        }

        private async Task SendWelcomeMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildWelcomeMessage(user.Language, messenger.Type);

            await messageSender.SendMessage(user.Id, messenger, message);
        }

        private async Task InsertMessengerWithVerificationCode(
            int userId,
            Messenger messenger,
            string verificationCode)
        {
            var command = new InsertMessengerCommand(userId, messenger.Type, messenger.Username, verificationCode);

            await dynamicRepository.ExecuteAsync(command);
        }

        private static string RandomSixDigitsNumber()
            => new Random().Next(0, 999999).ToString("D6");

        private static DateTime GetCurrentDateTime()
            => DateTime.Now;

        public Task<IEnumerable<Function>> GetAllFunctions()
            => dynamicRepository.FetchAsync<Function>(new GetAllFunctionsCriteria());

        public async Task<User> GetUser(int custId, int subAccountId, string language)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(language), language);

            var userDto = await dynamicRepository.GetAsync<UserDto>(new GetUserCriteria(custId, subAccountId));

            if (userDto == null)
            {
                return null;
            }

            var messengerAccounts = await GetMessengers(userDto.UserId);
            var functions = Enumerable.Empty<Function>();

            if (messengerAccounts.Any(messenger => messenger.Enabled))
            {
                functions = await GetFunctions(userDto.UserId, language);
            }

            return new User(
                userDto.UserId,
                userDto.CustId,
                userDto.SubAccountId,
                userDto.Language,
                messengerAccounts,
                functions);
        }

        public async Task<IEnumerable<Messenger>> GetMessengers(int userId, bool isEnabledOnly = false)
        {
            var messengers = await dynamicRepository.FetchAsync<Messenger>(new GetMessengerByUserCriteria(userId));

            if (isEnabledOnly)
            {
                return messengers.Where(messenger => messenger.Enabled);
            }

            foreach (var messenger in messengers)
            {
                if (messenger.IsVerifying)
                {
                    messenger.InputWrongVerificationCodeTimes = await GetFailedCount(userId, messenger);
                }
            }

            return messengers;
        }

        public async Task<User> GetUser(int userId)
        {
            ValidateInputParam(nameof(userId), userId);

            var userDto = await dynamicRepository.GetAsync<UserDto>(new GetUserCriteria(userId));

            if (userDto == null)
            {
                return default(User);
            }

            return new User(
                userDto.UserId,
                userDto.CustId,
                userDto.SubAccountId,
                userDto.Language,
                Enumerable.Empty<Messenger>(),
                Enumerable.Empty<Function>());
        }

        public async Task<IEnumerable<Function>> GetFunctions(int userId, string language = "")
        {
            ValidateInputParam(nameof(userId), userId);

            var functions = await dynamicRepository.FetchAsync<Function>(new GetUserFunctionsCriteria(userId));

            if (!string.IsNullOrWhiteSpace(language))
            {
                foreach (var function in functions)
                {
                    function.Name = await GetFunctionName(function.Id, language);
                }
            }

            return functions;
        }

        private async Task<string> GetFunctionName(int functionId, string language)
        {
            var functionLanguages = await cacheService.GetOrSetAsync(
                        string.Format(functionNameByLanguageCacheKey, functionId),
                        () => GetFunctionLanguage(functionId).Result,
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)));

            var functionName = functionLanguages
                .FirstOrDefault(languageDto => languageDto.LanguageCode == language)
                ?.FunctionName;

            if (string.IsNullOrWhiteSpace(functionName))
            {
                functionName = functionLanguages
                .FirstOrDefault(languageDto => languageDto.LanguageCode == DefaultLanguage)
                ?.FunctionName;
            }

            return functionName;
        }

        private Task<IEnumerable<FunctionLanguageDto>> GetFunctionLanguage(int functionId)
            => dynamicRepository.FetchAsync<FunctionLanguageDto>(new GetFunctionNameCriteria(functionId));

        public async Task UpdateFunctions(int userId, IEnumerable<Function> functions)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(functions), functions);

            var command = new UpdateUserFunctionsCommand(userId, functions);

            await dynamicRepository.ExecuteAsync(command);
        }

        public async Task UpdateLanguage(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            var command = new UpdateUserLanguageCommand(userId, language);

            await dynamicRepository.ExecuteAsync(command);
        }

        /// <summary>
        /// Need more refactor here
        /// </summary>
        public async Task<AddingUserResult> AddUser(int custId, int subAccountId, string custName, string language, Messenger messenger)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(language), language);
            ValidateInputParam(nameof(messenger), messenger);
            ValidateInputParam("Messenger Username", messenger.Username);
            ValidateInputParam("Messenger Type", messenger.Type);

            var userId = await InsertUser(custId, subAccountId, language, messenger);

            var result = await VerifyMessengerAccount(userId, messenger);
            if (result.IsNotOk)
            {
                return result;
            }

            await GenerateVerificationCode(userId, custName, messenger);

            return AddingUserResult.CreateSuccessfulAddingUserResult(userId);
        }

        private async Task<AddingUserResult> VerifyMessengerAccount(int userId, Messenger messenger)
        {
            if (await IsUserAddAccountOverMaxTimeInDay(userId, messenger.Type))
            {
                return AddingUserResult.CreateIsOverMaxAddAccountTimesResult();
            }

            if (await messageSender.IsMessengerAccountNotExisted(messenger))
            {
                return AddingUserResult.CreateAccountDoesNotExistResult();
            }

            if (await IsMessengerAccountBelongToAnotherUser(userId, messenger))
            {
                return AddingUserResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();
            }

            return AddingUserResult.CreateSuccessfulAddingUserResult();
        }

        private async Task<bool> IsMessengerAccountBelongToAnotherUser(int userId, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messenger), messenger);
            ValidateInputParam("Messenger Username", messenger.Username);
            ValidateInputParam("Messenger Type", messenger.Type);

            var messengerDto = await GetMessenger(messenger.Type, messenger.Username);

            return messengerDto != null
                && messengerDto.UserId != userId;
        }

        private async Task<int> InsertUser(int custId, int subAccountId, string language, Messenger messenger)
        {
            var command = new InsertUserCommand(custId, subAccountId, language);

            await dynamicRepository.ExecuteAsync(command);
            await IncreaseUserActionCount(command.UserId, chatbotConfigs.AddAccountActionId, messenger.Type, messenger.Username);

            return command.UserId;
        }

        private async Task<bool> IsUserAddAccountOverMaxTimeInDay(
            int userId,
            int messengerType)
        {
            var addUserLogActions = await GetUserAction(userId, messengerType, string.Empty, chatbotConfigs.AddAccountActionId);
            var numOfAddTime = addUserLogActions.Sum(action => action.CountTimes);

            return numOfAddTime > chatbotConfigs.MaxAddActionTimes;
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

        public async Task UpdateMessengerStatus(
            int userId,
            int messengerType,
            string messengerUsername,
            bool enabled)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            var command = new UpdateMessengerStatusCommand(userId, messengerType, messengerUsername, enabled);

            await dynamicRepository.ExecuteAsync(command);
        }

        public async Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messenger), messenger);
            ValidateInputParam("Messenger Username", messenger.Username);
            ValidateInputParam("Messenger Type", messenger.Type);

            var verificationResult = await CheckVerificationCode(userId, verificationCode, messenger);

            if (verificationResult.IsNotOk)
            {
                return verificationResult;
            }

            var user = await GetUser(userId);

            await Task.WhenAll(
                UpdateMessengerStatus(userId, messenger.Type, messenger.Username, true),
                ResetUserActionCount(userId, chatbotConfigs.WrongVerificationCodeActionId, messenger.Type, messenger.Username),
                SendWelcomeMessage(user, messenger));

            return VerificationCodeResult.CreateVerificationSuccessfulResult();
        }

        private async Task<VerificationCodeResult> CheckVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            if (await IsMessengerAccountBelongToAnotherUser(userId, messenger))
            {
                return VerificationCodeResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();
            }

            var criteria = new GetMessengerByUserCriteria(userId, messenger.Type, messenger.Username);
            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(criteria);
            var result = VerificationCodeResult.CreateVerificationSuccessfulResult();

            if (messengerDto == null
                || messengerDto.Username != messenger.Username)
            {
                result = VerificationCodeResult.CreateWrongVerificationCodeResult();
            }
            else if (messengerDto.GeneratedVerificationCodeTime.AddMinutes(chatbotConfigs.VerificationCodeExpiredTimeInMinutes) < getCurrentDateTimeFunc())
            {
                result = VerificationCodeResult.CreateVerificationCodeExpiredResult();
            }
            else if (messengerDto.VerificationCode != verificationCode)
            {
                result = VerificationCodeResult.CreateWrongVerificationCodeResult();
            }

            if (result.IsNotOk)
            {
                result = await LogVerificationCodeFailedAction(userId, messenger, result);
            }

            return result;
        }

        private async Task<VerificationCodeResult> LogVerificationCodeFailedAction(
            int userId,
            Messenger messenger,
            VerificationCodeResult result)
        {
            await IncreaseUserActionCount(
                                    userId,
                                    chatbotConfigs.WrongVerificationCodeActionId,
                                    messenger.Type,
                                    messenger.Username);

            return new VerificationCodeResult(
                result.Code,
                result.Message,
                await GetFailedCount(userId, messenger));
        }

        private async Task<int> GetFailedCount(int userId, Messenger messenger)
        {
            if (userId == 0 || messenger == null)
            {
                return 0;
            }

            var failedAction = (await GetUserAction(userId, messenger.Type, messenger.Username, chatbotConfigs.WrongVerificationCodeActionId)).FirstOrDefault();

            return failedAction?.CountTimes ?? 0;
        }

        private async Task IncreaseUserActionCount(
            int userId,
            int actionId,
            int messengerType,
            string messengerUsername)
        {
            var command = new InsertUserActionCommand(userId, actionId, messengerType, messengerUsername);

            await dynamicRepository.ExecuteAsync(command);
        }

        private async Task ResetUserActionCount(
            int userId,
            int actionId,
            int messengerType,
            string messengerUsername)
        {
            var command = new ResetActionCountTimesCommand(userId, actionId, messengerType, messengerUsername);

            await dynamicRepository.ExecuteAsync(command);
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

        public async Task<MessengerDto> GetMessenger(int messengerType, string messengerUsername)
        {
            var criteria = new GetMessengerCriteria(messengerUsername, messengerType);

            var messenger = await dynamicRepository.GetAsync<MessengerDto>(criteria);

            return messenger;
        }

        public async Task<IEnumerable<User>> GetUsersByFunction(int functionId, int environment)
        {
            var criteria = new GetUsersByFunctionCriteria(functionId, environment);

            var users = await dynamicRepository.FetchAsync<User>(criteria);

            return users;
        }
    }
}