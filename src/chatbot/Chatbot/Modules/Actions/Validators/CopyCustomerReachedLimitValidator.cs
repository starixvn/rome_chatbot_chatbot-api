namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;

    public class CopyCustomerReachedLimitValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CopyCustomerService copyCustomerService;
        private readonly UserLogActionService userLogActionService;
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyCustomerReachedLimitValidator(
            IMessageBuilder messageBuilder,
            UserLogActionService userLogActionService,
            CopyCustomerService copyCustomerService,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageBuilder = messageBuilder;
            this.chatbotConfigs = chatbotConfigs;
            this.copyCustomerService = copyCustomerService;
            this.userLogActionService = userLogActionService;
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

            var isReachedLimitOfCopyCustomer = userLogActionService.IsUserReachedTheLimitOfActionInDay(
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