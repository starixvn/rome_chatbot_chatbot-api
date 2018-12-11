namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;

    public class MaxCreditValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CopyCustomerService copyCustomerService;

        public MaxCreditValidator(
            IMessageBuilder messageBuilder,
            CopyCustomerService copyCustomerService)
        {
            this.messageBuilder = messageBuilder;
            this.copyCustomerService = copyCustomerService;
        }

        public ActionValidationResult ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger)
        {
            if (!string.IsNullOrWhiteSpace(userResponse)
                && decimal.TryParse(userResponse, out decimal number))
            {
                var pt = copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel)
                    .GetAwaiter().GetResult();

                if (number < 0 || number > pt.MaxCredit)
                {
                    return ActionValidationResult.CreateErrorValidationResult(
                        messageBuilder.BuildInvalidMaxCreditMessage(user.CustLevel - 1, pt.MaxCredit, user.Language));
                }

                return ActionValidationResult.CreateSuccessfulValidationResult();
            }

            return ActionValidationResult.CreateErrorValidationResult(messageBuilder.BuildInvalidNumericMessage(userResponse, user.Language));
        }
    }
}