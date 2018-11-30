namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Shared.Configurations;

    public class CopyAgentConfirmationTitleBuilder : IActionTitleBuilder
    {
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IMessageBuilder messageBuilder;

        public CopyAgentConfirmationTitleBuilder(
            IChatbotConfigs chatbotConfigs,
            IMessageBuilder messageBuilder)
        {
            this.chatbotConfigs = chatbotConfigs;
            this.messageBuilder = messageBuilder;
        }

        public string BuildActionTitle(User user, IList<UserAction> userActions)
        {
            var credit = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputCreditActionId)?.UserResponse;
            var username = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputUsernameActionId)?.UserResponse;

            return messageBuilder.BuildCopyCustomerConfirmationTitle(user.CustLevel - 1, credit, username, user.Language);
        }
    }
}