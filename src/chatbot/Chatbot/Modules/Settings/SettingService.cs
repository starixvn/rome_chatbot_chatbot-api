namespace Chatbot.Modules.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Helpers;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int userId, string language = "");

        Task UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task UpdateLanguage(int userId, string language);

        Task GenerateVerificationCode(int userId, Messenger messenger);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string custName, string language, Messenger messenger, int custLevel);

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
        private readonly MessageService messageService;

        public SettingServiceImpl(
            IDynamicRepository dynamicRepository,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            ICacheService cacheService,
            IMessageLogger messageLogger,
            IChatbotConfigs chatbotConfigs,
            MessageService messageService,
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
            this.messageService = messageService;
        }

        public async Task GenerateVerificationCode(int userId, Messenger messenger)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var verificationCode = randomNumberFunc();
            var user = await GetUser(userId);

            await Task.WhenAll(
                InsertMessengerWithVerificationCode(userId, messenger, verificationCode),
                SendVerificationCode(user, messenger, verificationCode));
        }

        private async Task SendVerificationCode(User user, Messenger messenger, string verificationCode)
        {
            var message = messageBuilder.BuildVerificationCodeMessage(
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
            InputValidator.Validate(nameof(custId), custId);
            InputValidator.Validate(nameof(language), language);

            var user = await dynamicRepository.GetAsync<User>(new GetUserCriteria(custId, subAccountId));

            if (user == null)
            {
                return null;
            }

            var messengerAccounts = await GetMessengers(user.Id);
            var functions = Enumerable.Empty<Function>();

            if (messengerAccounts.Any(messenger => messenger.Enabled))
            {
                functions = await GetFunctions(user.Id, language);
            }

            return new User(
                user.Id,
                user.CustId,
                user.CustLevel,
                user.SubAccountId,
                user.Language,
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
            InputValidator.Validate(nameof(userId), userId);

            var user = await dynamicRepository.GetAsync<User>(new GetUserCriteria(userId));

            if (user == null)
            {
                return default(User);
            }

            return new User(
                user.Id,
                user.CustId,
                user.CustLevel,
                user.SubAccountId,
                user.Language,
                Enumerable.Empty<Messenger>(),
                Enumerable.Empty<Function>());
        }

        public async Task<IEnumerable<Function>> GetFunctions(int userId, string language = "")
        {
            InputValidator.Validate(nameof(userId), userId);

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
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(functions), functions);

            await UpdateLastQueryTime(userId, functions);

            var command = new UpdateUserFunctionsCommand(userId, functions);

            await dynamicRepository.ExecuteAsync(command);
        }

        public async Task UpdateLanguage(int userId, string language)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(language), language);

            var command = new UpdateUserLanguageCommand(userId, language);

            await dynamicRepository.ExecuteAsync(command);
        }

        /// <summary>
        /// Need more refactor here
        /// </summary>
        public async Task<AddingUserResult> AddUser(int custId, int subAccountId, string custName, string language, Messenger messenger, int custLevel)
        {
            InputValidator.Validate(nameof(custId), custId);
            InputValidator.Validate(nameof(custLevel), custLevel);
            InputValidator.Validate(nameof(language), language);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var userId = await InsertUser(custId, subAccountId, language, messenger, custLevel);

            var result = await VerifyMessengerAccount(userId, messenger);
            if (result.IsNotOk)
            {
                return result;
            }

            await GenerateVerificationCode(userId, messenger);

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
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var messengerDto = await GetMessenger(messenger.Type, messenger.Username);

            return messengerDto != null
                && messengerDto.UserId != userId;
        }

        private async Task<int> InsertUser(int custId, int subAccountId, string language, Messenger messenger, int custLevel)
        {
            var command = new InsertUserCommand(custId, subAccountId, language, custLevel);

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
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(messengerType), messengerType);

            var command = new UpdateMessengerStatusCommand(userId, messengerType, messengerUsername, enabled);

            await dynamicRepository.ExecuteAsync(command);
        }

        public async Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(verificationCode), verificationCode);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var verificationResult = await CheckVerificationCode(userId, verificationCode, messenger);

            if (verificationResult.IsNotOk)
            {
                return verificationResult;
            }

            var user = await GetUser(userId);

            await Task.WhenAll(
                UpdateMessengerStatus(userId, messenger.Type, messenger.Username, true),
                ResetUserActionCount(userId, chatbotConfigs.WrongVerificationCodeActionId, messenger.Type, messenger.Username),
                SendWelcomeMessage(user, messenger),
                UpdateLastQueryTime(userId, null));

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

        private async Task UpdateLastQueryTime(int userId, IEnumerable<Function> functions)
        {
            var userFunctions = functions ?? (await GetFunctions(userId));

            foreach (var function in userFunctions)
            {
                if (function.Enabled && chatbotConfigs.MessageAndAnnoucementFunctionIds.Contains(function.Id))
                {
                    await messageService.UpdateLastQueryTime(userId, function.Id, 0, DateTime.Now);
                }
            }
        }
    }
}