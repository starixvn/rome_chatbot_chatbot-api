namespace Chatbot.Modules.UnderMaintenance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;
    using Fanex.Logging;
    using MessengerClient = Messenger.Client;

    public interface UnderMaintenanceService
    {
        Task<Result> SendCompleteUMNotifications();
    }

    public class UnderMaintenanceServiceImpl : UnderMaintenanceService
    {
        private readonly SettingService settingService;
        private readonly ActionService actionService;
        private readonly CustomerService customerService;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly ILogger logger;

        public UnderMaintenanceServiceImpl(
            SettingService settingService,
            ActionService actionService,
            CustomerService customerService,
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IChatbotConfigs chatbotConfigs,
            ILogger logger)
        {
            this.settingService = settingService;
            this.actionService = actionService;
            this.customerService = customerService;
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.chatbotConfigs = chatbotConfigs;
            this.logger = logger;
        }

        public async Task<Result> SendCompleteUMNotifications()
        {
            logger.Info("Start SendCompleteUMNotifications");
            var users = await settingService.GetUsers().ConfigureAwait(false);

            if (users == null || !users.Any())
            {
                return Result.CreateSuccessfulResult();
            }

            foreach (var user in users)
            {
                if (await IsCustomerClosed(user).ConfigureAwait(false))
                {
                    continue;
                }

                var messengers = await settingService.GetMessengers(user.Id, true).ConfigureAwait(false);

                if (messengers == null || !messengers.Any())
                {
                    continue;
                }

                user.Functions = await settingService.GetFunctions(user.Id).ConfigureAwait(false);

                var rootActions = await actionService.GetRootActions(user).ConfigureAwait(false);

                foreach (var messenger in messengers)
                {
                    if (chatbotConfigs.AllowSendCompleteUMNotificationMessengerUsernames.Contains("*")
                        || chatbotConfigs.AllowSendCompleteUMNotificationMessengerUsernames.Contains(messenger.Username, StringComparer.OrdinalIgnoreCase))
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        Task.Run(() => SendCompleteUMMessages(user, messenger, rootActions));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                }
            }
            logger.Info("End SendCompleteUMNotifications");
            return Result.CreateSuccessfulResult();
        }

        private Task<bool> IsCustomerClosed(User user)
            => customerService.IsAccountClosed(user.CustId, user.SubAccountId);

        private async Task SendCompleteUMMessages(User user, Messenger messenger, IEnumerable<MessengerClient.Models.MessageAction> rootActions)
        {
            await BuildAndSendCompleteUMNotification(user, messenger).ConfigureAwait(false);

            if (rootActions != null && rootActions.Any())
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
                await BuildAndSendRootActions(user, messenger, rootActions).ConfigureAwait(false);
            }
        }

        private async Task BuildAndSendCompleteUMNotification(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildCompleteUMNotification(messenger.Type, user.Language);

            await messageSender.SendMessage(user.Id, messenger, message, user.SelfdestructionPeriod).ConfigureAwait(false);
        }

        private async Task BuildAndSendRootActions(User user, Messenger messenger, IEnumerable<MessengerClient.Models.MessageAction> rootActions)
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
}