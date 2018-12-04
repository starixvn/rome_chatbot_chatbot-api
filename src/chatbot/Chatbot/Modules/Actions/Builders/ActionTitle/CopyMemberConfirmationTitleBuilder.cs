namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Helpers;

    public class CopyMemberConfirmationTitleBuilder : IActionTitleBuilder
    {
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyMemberConfirmationTitleBuilder(IChatbotConfigs chatbotConfigs)
        {
            this.chatbotConfigs = chatbotConfigs;
        }

        public string BuildActionTitle(User user, IList<UserAction> userActions)
        {
            var credit = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyMemberInputCreditActionId)?.UserResponse;
            var username = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyMemberInputUsernameActionId)?.UserResponse;

            return string.Format(StaticResources.Get(nameof(Languages.CopyMemberComfirmation), user.Language), credit, username);
        }
    }
}