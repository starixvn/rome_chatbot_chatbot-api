namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;

    public class CopyCustomerReachedLimitValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly SettingService settingService;
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyCustomerReachedLimitValidator(
            IMessageBuilder messageBuilder,
            SettingService settingService,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageBuilder = messageBuilder;
            this.settingService = settingService;
            this.chatbotConfigs = chatbotConfigs;
        }

        public Result ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger)
        {
            var isReachedLimitOfCopyCustomer = settingService.IsUserReachedTheLimitOfActionInDay(
                user.Id,
                messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .GetAwaiter()
                .GetResult();

            if (isReachedLimitOfCopyCustomer)
            {
                return Result.CreateErrorResult(
                    messageBuilder.BuildCopyCustomerReachedLimitMessage(
                        chatbotConfigs.CopyCustomerLimitPerDay,
                        user.Language));
            }

            return Result.CreateSuccessfulResult();
        }
    }
}