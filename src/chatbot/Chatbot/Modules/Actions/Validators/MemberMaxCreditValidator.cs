namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Shared.Configurations;

    public class MemberMaxCreditValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IChatbotConfigs chatbotConfigs;

        public MemberMaxCreditValidator(
            IMessageBuilder messageBuilder,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageBuilder = messageBuilder;
            this.chatbotConfigs = chatbotConfigs;
        }

        public Task<ActionValidationResult> ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger)
        {
            if (!string.IsNullOrWhiteSpace(userResponse)
                && decimal.TryParse(userResponse, out decimal number))
            {
                var previousResponse = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputCreditActionId)?.UserResponse;

                var agentMaxCredit = decimal.Parse(previousResponse);

                if (number < 0 || number > agentMaxCredit)
                {
                    return Task.FromResult(ActionValidationResult.CreateErrorValidationResult(
                        messageBuilder.BuildInvalidMaxCreditMessage(CustLevel.MemberLevel, agentMaxCredit, user.Language)));
                }

                return Task.FromResult(ActionValidationResult.CreateSuccessfulValidationResult());
            }

            return Task.FromResult(ActionValidationResult.CreateErrorValidationResult(
                messageBuilder.BuildInvalidNumericMessage(userResponse, user.Language)));
        }
    }
}