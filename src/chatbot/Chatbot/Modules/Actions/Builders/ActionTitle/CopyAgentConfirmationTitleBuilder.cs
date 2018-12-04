namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Helpers;

    public class CopyAgentConfirmationTitleBuilder : IActionTitleBuilder
    {
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyAgentConfirmationTitleBuilder(IChatbotConfigs chatbotConfigs)
        {
            this.chatbotConfigs = chatbotConfigs;
        }

        public string BuildActionTitle(User user, IList<UserAction> userActions)
        {
            var credit = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputCreditActionId)?.UserResponse;
            var maxMemberCredit = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputMemberCreditActionId)?.UserResponse;
            var username = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputUsernameActionId)?.UserResponse;

            return string.Format(
                StaticResources.Get(nameof(Languages.CopyAgentComfirmation), user.Language),
                credit,
                maxMemberCredit,
                username);
        }
    }
}