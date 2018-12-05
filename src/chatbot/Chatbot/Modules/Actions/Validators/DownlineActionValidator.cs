namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;

    public class DownlineActionValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CustomerService customerService;

        public DownlineActionValidator(
            IMessageBuilder messageBuilder,
            CustomerService customerService)
        {
            this.messageBuilder = messageBuilder;
            this.customerService = customerService;
        }

        public ActionValidationResult ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger)
        {
            if (!string.IsNullOrWhiteSpace(userResponse))
            {
                var downlineInfo = customerService.GetDownlineInfo(user.CustId, userResponse)
                    .GetAwaiter().GetResult();

                if (downlineInfo != null && downlineInfo.CustId > 0)
                {
                    return ActionValidationResult.CreateSuccessfulValidationResult();
                }
            }

            return ActionValidationResult.CreateErrorValidationResult(
                messageBuilder.BuildInvalidDownlineMessage(userResponse, user.Language));
        }
    }
}