namespace Chatbot.Modules.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Modules.SettingLogs;
    using Chatbot.Modules.SettingLogs.Models;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Helpers;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using MessengerClient = Messenger.Client;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int userId, string language = "");

        Task UpdateFunctions(int userId, IEnumerable<Function> functions, UpdateFunctionsLog logData);

        Task UpdateLanguage(int userId, string language, UpdateLanguageLog logData);

        Task GenerateVerificationCode(int userId, Messenger messenger);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger, VerifyAccountLog logData);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string custName, string language, Messenger messenger, int custLevel, LinkAccountLog logData);

        Task<IEnumerable<Function>> GetAllFunctions(string language = "");

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<User> GetUser(int userId);

        Task UpdateMessengerStatus(int userId, int messengerType, string messengerUsername, bool enabled);

        Task<MessengerDto> GetMessenger(int messengerType, string messengerUsername);

        Task<IEnumerable<User>> GetUsersByFunction(int functionId, int environment);

        Task<IEnumerable<Messenger>> GetMessengers(int userId, bool isEnabledOnly = false);

        Task Unsubscribe(int userId, Messenger messenger, UnlinkAccountLog logData);

        Task UpdateSelfdestructionPeriod(int userId, int minutes, UpdateSelfdestructionPeriodLog logData);

        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUser(int messengerType, string messengerUsername);
    }

    public class SettingServiceImpl : SettingService
    {
        private const string AllFunctionsCacheKey = "SettingServiceImpl.AllFunctionsCacheKey";
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<string> randomNumberFunc;
        private readonly Func<DateTime> getCurrentDateTimeFunc;
        private readonly ICacheService cacheService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly SettingLogService settingLogService;
        private readonly ResourceService resourceService;
        private readonly ActionService actionService;
        private readonly UserLogActionService userLogActionService;

#pragma warning disable S107 // Methods should not have too many parameters

        public SettingServiceImpl(
            IDynamicRepository dynamicRepository,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            ICacheService cacheService,
            IChatbotConfigs chatbotConfigs,
            SettingLogService settingLogService,
            ResourceService resourceService,
            ActionService actionService,
            UserLogActionService userLogActionService,
            Func<string> randomNumberFunc = null,
            Func<DateTime> getCurrentDateTimeFunc = null)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.dynamicRepository = dynamicRepository;
            this.randomNumberFunc = randomNumberFunc ?? RandomSixDigitsNumber;
            this.cacheService = cacheService ?? new CacheService();
            this.getCurrentDateTimeFunc = getCurrentDateTimeFunc ?? GetCurrentDateTime;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.chatbotConfigs = chatbotConfigs;
            this.settingLogService = settingLogService;
            this.resourceService = resourceService;
            this.actionService = actionService;
            this.userLogActionService = userLogActionService;
        }

        public async Task GenerateVerificationCode(int userId, Messenger messenger)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var verificationCode = randomNumberFunc();
            var user = await GetUser(userId).ConfigureAwait(false);

            await Task.WhenAll(
                InsertMessengerWithVerificationCode(userId, messenger, verificationCode),
                SendVerificationCode(user, messenger, verificationCode)).ConfigureAwait(false);
        }

        private async Task SendVerificationCode(User user, Messenger messenger, string verificationCode)
        {
            var message = messageBuilder.BuildVerificationCodeMessage(
                user.Language,
                messenger.Type,
                verificationCode,
                chatbotConfigs.VerificationCodeExpiredTimeInMinutes);

            await messageSender.SendMessage(user.Id, messenger, message, user.SelfdestructionPeriod).ConfigureAwait(false);
        }

        private async Task SendWelcomeMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildWelcomeMessage(user.Language, messenger.Type);

            await messageSender.SendMessage(user.Id, messenger, message, user.SelfdestructionPeriod).ConfigureAwait(false);
        }

        private async Task SendSuggestionActions(User user, Messenger messenger)
        {
            var rootActions = await actionService.GetRootActions(user).ConfigureAwait(false);

            if (rootActions != null && rootActions.Any())
            {
                var message = new MessengerClient.Models.Message
                {
                    Content = messageBuilder.BuildSuggestionActionsTitle(user.Language),
                    MessageMarkup = new MessengerClient.Models.MessageMarkup
                    {
                        MessageActions = rootActions
                    }
                };

                await messageSender.SendMessage(user.Id, messenger, message, user.SelfdestructionPeriod).ConfigureAwait(false);
            }
        }

        private async Task InsertMessengerWithVerificationCode(
            int userId,
            Messenger messenger,
            string verificationCode)
        {
            var command = new InsertMessengerCommand(userId, messenger.Type, messenger.Username, verificationCode);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        }

        private static string RandomSixDigitsNumber()
            => new Random().Next(0, 999999).ToString("D6");

        private static DateTime GetCurrentDateTime()
            => DateTime.Now;

        public async Task<IEnumerable<Function>> GetAllFunctions(string language = "")
        {
            var functions = await cacheService.GetOrSetAsync(
                AllFunctionsCacheKey,
                LoadAllFunctions,
                new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)))
            .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(language))
            {
                foreach (var function in functions)
                {
                    function.Name = await GetFunctionName(function.Id, language).ConfigureAwait(false);
                }
            }

            return functions;
        }

        private IEnumerable<Function> LoadAllFunctions()
        {
            var functions = dynamicRepository.FetchAsync<Function>(new GetAllFunctionsCriteria()).Result;

            return functions.Where(function
                => !chatbotConfigs.IsProduction
                    || function.Environment == Shared.Constants.Environment.PRO);
        }

        public async Task<User> GetUser(int custId, int subAccountId, string language)
        {
            InputValidator.Validate(nameof(custId), custId);
            InputValidator.Validate(nameof(language), language);

            var user = await dynamicRepository
                .GetAsync<User>(new GetUserCriteria(custId, subAccountId))
                .ConfigureAwait(false);

            if (user == null)
            {
                return null;
            }

            var messengerAccounts = await GetMessengers(user.Id).ConfigureAwait(false);
            var functions = Enumerable.Empty<Function>();

            if (messengerAccounts.Any(messenger => messenger.Enabled))
            {
                functions = await GetFunctions(user.Id, language).ConfigureAwait(false);
            }

            return new User(
                user.Id,
                user.CustId,
                user.CustLevel,
                user.SubAccountId,
                user.Language,
                user.SelfdestructionPeriod,
                messengerAccounts,
                functions);
        }

        public async Task<IEnumerable<Messenger>> GetMessengers(int userId, bool isEnabledOnly = false)
        {
            var messengers = await dynamicRepository
                .FetchAsync<Messenger>(new GetMessengerByUserCriteria(userId))
                .ConfigureAwait(false);

            if (isEnabledOnly)
            {
                return messengers.Where(messenger => messenger.Enabled);
            }

            foreach (var messenger in messengers)
            {
                if (messenger.IsVerifying)
                {
                    messenger.InputWrongVerificationCodeTimes = await GetFailedCount(userId, messenger).ConfigureAwait(false);
                }
            }

            return messengers;
        }

        public async Task<User> GetUser(int userId)
        {
            InputValidator.Validate(nameof(userId), userId);

            var user = await dynamicRepository
                .GetAsync<User>(new GetUserCriteria(userId))
                .ConfigureAwait(false);

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
                user.SelfdestructionPeriod,
                Enumerable.Empty<Messenger>(),
                Enumerable.Empty<Function>());
        }

        public async Task<IEnumerable<Function>> GetFunctions(int userId, string language = "")
        {
            InputValidator.Validate(nameof(userId), userId);

            var functions = await GetAllFunctions().ConfigureAwait(false);
            var userFunctions = (await dynamicRepository
                .FetchAsync<Function>(new GetUserFunctionsCriteria(userId))
                .ConfigureAwait(false))
                .Where(userFunction => functions.Any(func => func.Id == userFunction.Id));

            if (!string.IsNullOrWhiteSpace(language))
            {
                foreach (var function in userFunctions)
                {
                    function.Name = await GetFunctionName(function.Id, language).ConfigureAwait(false);
                }
            }

            return userFunctions;
        }

        private async Task<string> GetFunctionName(int functionId, string language)
            => (await resourceService.GetResource(functionId, ResourceType.Function, language).ConfigureAwait(false))?.Value;

        public async Task UpdateFunctions(int userId, IEnumerable<Function> functions, UpdateFunctionsLog logData)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(functions), functions);

            var command = new UpdateUserFunctionsCommand(userId, functions);
            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

            settingLogService.WriteLog(logData);
        }

        public async Task UpdateLanguage(int userId, string language, UpdateLanguageLog logData)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(language), language);

            var command = new UpdateUserLanguageCommand(userId, language);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

            settingLogService.WriteLog(logData);
        }

        /// <summary>
        /// Need more refactor here
        /// </summary>
        public async Task<AddingUserResult> AddUser(int custId, int subAccountId, string custName, string language, Messenger messenger, int custLevel, LinkAccountLog logData)
        {
            InputValidator.Validate(nameof(custId), custId);
            InputValidator.Validate(nameof(custLevel), custLevel);
            InputValidator.Validate(nameof(language), language);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var user = await GetUser(custId, subAccountId, language).ConfigureAwait(false);

            var userId = user == null
                ? await InsertUser(custId, subAccountId, language, messenger, custLevel).ConfigureAwait(false)
                : user.Id;

            var result = await VerifyMessengerAccount(userId, messenger).ConfigureAwait(false);
            if (result.IsNotOk)
            {
                return result;
            }

            settingLogService.WriteLog(logData);

            await GenerateVerificationCode(userId, messenger).ConfigureAwait(false);

            return AddingUserResult.CreateSuccessfulAddingUserResult(userId);
        }

        private async Task<AddingUserResult> VerifyMessengerAccount(int userId, Messenger messenger)
        {
            if (await IsUserAddAccountOverMaxTimeInDay(userId, messenger.Type).ConfigureAwait(false))
            {
                return AddingUserResult.CreateIsOverMaxAddAccountTimesResult();
            }

            if (await messageSender.IsMessengerAccountNotExisted(messenger).ConfigureAwait(false))
            {
                return AddingUserResult.CreateAccountDoesNotExistResult();
            }

            if (await IsMessengerAccountBelongToAnotherUser(userId, messenger).ConfigureAwait(false)
                || await IsUserAlreadySettingUpAccount(userId).ConfigureAwait(false))
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

            var messengerDto = await GetMessenger(messenger.Type, messenger.Username).ConfigureAwait(false);

            return messengerDto != null && messengerDto.UserId > 0;
        }

        private async Task<bool> IsUserAlreadySettingUpAccount(int userId)
        {
            InputValidator.Validate(nameof(userId), userId);

            var messengerDto = await GetMessengers(userId, true).ConfigureAwait(false);

            return messengerDto.Any();
        }

        private async Task<int> InsertUser(int custId, int subAccountId, string language, Messenger messenger, int custLevel)
        {
            var command = new InsertUserCommand(custId, subAccountId, language, custLevel);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
            await userLogActionService.IncreaseUserActionCount(command.UserId, chatbotConfigs.AddAccountActionId, messenger.Type, messenger.Username)
                .ConfigureAwait(false);

            return command.UserId;
        }

        private async Task<bool> IsUserAddAccountOverMaxTimeInDay(int userId, int messengerType)
            => await userLogActionService.IsUserReachedTheLimitOfActionInDay(
                userId,
                messengerType,
                chatbotConfigs.AddAccountActionId,
                chatbotConfigs.MaxAddActionTimes).ConfigureAwait(false);

        public async Task UpdateMessengerStatus(
            int userId,
            int messengerType,
            string messengerUsername,
            bool enabled)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(messengerType), messengerType);

            var command = new UpdateMessengerStatusCommand(userId, messengerType, messengerUsername, enabled);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        }

        public async Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger, VerifyAccountLog logData)
        {
            InputValidator.Validate(nameof(userId), userId);
            InputValidator.Validate(nameof(verificationCode), verificationCode);
            InputValidator.Validate(nameof(messenger), messenger);
            InputValidator.Validate("Messenger Username", messenger.Username);
            InputValidator.Validate("Messenger Type", messenger.Type);

            var verificationResult = await CheckVerificationCode(userId, verificationCode, messenger).ConfigureAwait(false);

            if (verificationResult.IsNotOk)
            {
                return verificationResult;
            }

            var user = await GetUser(userId).ConfigureAwait(false);
            user.Functions = await GetFunctions(userId).ConfigureAwait(false);

            await Task.WhenAll(
                UpdateMessengerStatus(userId, messenger.Type, messenger.Username, true),
                ResetUserActionCount(userId, chatbotConfigs.WrongVerificationCodeActionId, messenger.Type, messenger.Username),
                SendWelcomeMessage(user, messenger))
                .ConfigureAwait(false);

            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            await SendSuggestionActions(user, messenger).ConfigureAwait(false);

            settingLogService.WriteLog(logData);

            return VerificationCodeResult.CreateVerificationSuccessfulResult();
        }

        private async Task<VerificationCodeResult> CheckVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            if (await IsMessengerAccountBelongToAnotherUser(userId, messenger).ConfigureAwait(false))
            {
                return VerificationCodeResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();
            }

            var criteria = new GetMessengerByUserCriteria(userId, messenger.Type, messenger.Username);
            var messengerDto = await dynamicRepository.GetAsync<MessengerDto>(criteria).ConfigureAwait(false);
            VerificationCodeResult result;

            if (messengerDto == null
                || messengerDto.Username.ToLowerInvariant() != messenger.Username.ToLowerInvariant())
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
            else
            {
                result = VerificationCodeResult.CreateVerificationSuccessfulResult();
            }

            if (result.IsNotOk)
            {
                result = await LogVerificationCodeFailedAction(userId, messenger, result).ConfigureAwait(false);
            }

            return result;
        }

        private async Task<VerificationCodeResult> LogVerificationCodeFailedAction(
            int userId,
            Messenger messenger,
            VerificationCodeResult result)
        {
            await userLogActionService.IncreaseUserActionCount(
                                    userId,
                                    chatbotConfigs.WrongVerificationCodeActionId,
                                    messenger.Type,
                                    messenger.Username)
                                    .ConfigureAwait(false);

            return new VerificationCodeResult(
                result.Code,
                result.Message,
                await GetFailedCount(userId, messenger).ConfigureAwait(false));
        }

        private async Task<int> GetFailedCount(int userId, Messenger messenger)
        {
            if (userId == 0 || messenger == null)
            {
                return 0;
            }

            var failedAction = (await userLogActionService.GetUserAction(userId, messenger.Type, messenger.Username, chatbotConfigs.WrongVerificationCodeActionId).ConfigureAwait(false))
                .FirstOrDefault();

            return failedAction?.CountTimes ?? 0;
        }

        private async Task ResetUserActionCount(
            int userId,
            int actionId,
            int messengerType,
            string messengerUsername)
        {
            var command = new ResetActionCountTimesCommand(userId, actionId, messengerType, messengerUsername);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        }

        public async Task<MessengerDto> GetMessenger(int messengerType, string messengerUsername)
        {
            var criteria = new GetMessengerCriteria(messengerUsername, messengerType);

            var messenger = await dynamicRepository.GetAsync<MessengerDto>(criteria).ConfigureAwait(false);

            return messenger;
        }

        public async Task<IEnumerable<User>> GetUsersByFunction(int functionId, int environment)
        {
            var criteria = new GetUsersByFunctionCriteria(functionId, environment);

            var users = await dynamicRepository.FetchAsync<User>(criteria).ConfigureAwait(false);

            return users;
        }

        public async Task Unsubscribe(int userId, Messenger messenger, UnlinkAccountLog logData)
        {
            var isVerified = await IsUserAlreadySettingUpAccount(userId).ConfigureAwait(false);

            await UpdateMessengerStatus(userId, messenger.Type, messenger.Username, false).ConfigureAwait(false);

            settingLogService.WriteLog(logData);

            if (isVerified)
            {
                await SendUnlinkMessage(userId, messenger).ConfigureAwait(false);
            }
        }

        private async Task SendUnlinkMessage(int userId, Messenger messenger)
        {
            var user = await GetUser(userId).ConfigureAwait(false);

            var message = messageBuilder.BuildUnlinkMessage(user.Language);

            await messageSender.SendMessage(user.Id, messenger, message, user.SelfdestructionPeriod).ConfigureAwait(false);
        }

        public async Task UpdateSelfdestructionPeriod(int userId, int minutes, UpdateSelfdestructionPeriodLog logData)
        {
            var command = new UpdateSelfdestructionPeriodCommand(userId, minutes);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

            settingLogService.WriteLog(logData);
        }

        public async Task<IEnumerable<User>> GetUsers()
            => await dynamicRepository.FetchAsync<User>(new GetAllUsersCriteria()).ConfigureAwait(false);
        }

        public async Task<User> GetUser(int messengerType, string messengerUsername)
        {
            var messenger = await GetMessenger(messengerType, messengerUsername);

            if (messenger == null
                || messenger.UserId < 0)
            {
                return new User();
            }

            return await GetUser(messenger.UserId);
    }
}