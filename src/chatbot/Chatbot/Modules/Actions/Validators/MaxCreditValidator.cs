namespace Chatbot.Modules.Actions.Validators
{
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
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

        public Result ValidateAction(int actionId, User user, string userResponse)
        {
            if (!string.IsNullOrWhiteSpace(userResponse)
                && int.TryParse(userResponse, out int number))
            {
                var pt = copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel)
                    .GetAwaiter().GetResult();

                if (number > pt.MaxCredit)
                {
                    return Result.CreateErrorResult(messageBuilder.BuildInvalidMaxCreditMessage(user.CustLevel - 1, pt.MaxCredit, user.Language));
                }

                return Result.CreateSuccessfulResult();
            }

            return Result.CreateErrorResult(messageBuilder.BuildInvalidNumericMessage(userResponse, user.Language));
        }
    }
}