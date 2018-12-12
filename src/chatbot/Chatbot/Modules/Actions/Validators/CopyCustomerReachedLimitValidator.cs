namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;

    public class CopyCustomerReachedLimitValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly SettingService settingService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyCustomerReachedLimitValidator(
            IMessageBuilder messageBuilder,
            SettingService settingService,
            CopyCustomerService copyCustomerService,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageBuilder = messageBuilder;
            this.settingService = settingService;
            this.chatbotConfigs = chatbotConfigs;
            this.copyCustomerService = copyCustomerService;
        }

        public ActionValidationResult ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger)
        {
            if (user.CustLevel == CustLevel.AgentLevel)
            {
                var validateResult = copyCustomerService.ValidateMaxNewMember(user.CustId, user.Language)
                    .GetAwaiter().GetResult();

                if (!validateResult.IsSuccess)
                {
                    return ActionValidationResult.CreateErrorValidationResult(validateResult.Message, true);
                }
            }

            var isReachedLimitOfCopyCustomer = settingService.IsUserReachedTheLimitOfActionInDay(
                user.Id,
                messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .GetAwaiter()
                .GetResult();

            if (isReachedLimitOfCopyCustomer)
            {
                return ActionValidationResult.CreateErrorValidationResult(
                    messageBuilder.BuildCopyCustomerReachedLimitMessage(
                        chatbotConfigs.CopyCustomerLimitPerDay,
                        user.Language),
                    true);
            }

            return ActionValidationResult.CreateSuccessfulValidationResult();
        }
    }
}