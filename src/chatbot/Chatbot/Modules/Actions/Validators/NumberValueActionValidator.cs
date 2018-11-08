namespace Chatbot.Modules.Actions.Validators
{
    using Chatbot.Models;
    using Chatbot.Modules.MessageBuilders;

    public class NumberValueActionValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;

        public NumberValueActionValidator(IMessageBuilder messageBuilder)
        {
            this.messageBuilder = messageBuilder;
        }

        public Result ValidateAction(int actionId, User user, string userResponse)
        {
            if (!string.IsNullOrWhiteSpace(userResponse)
                && int.TryParse(userResponse, out int number))
            {
                return Result.CreateSuccessfulResult();
            }

            return Result.CreateErrorResult(messageBuilder.BuildInvalidNumerOfDownline(user.Language));
        }
    }
}