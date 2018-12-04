namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Helpers;

    public class CopyMasterConfirmationTitleBuilder : IActionTitleBuilder
    {
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyMasterConfirmationTitleBuilder(IChatbotConfigs chatbotConfigs)
        {
            this.chatbotConfigs = chatbotConfigs;
        }

        public string BuildActionTitle(User user, IList<UserAction> userActions)
        {
            var credit = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyMasterInputCreditActionId)?.UserResponse;
            var username = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyMasterInputUsernameActionId)?.UserResponse;

            return string.Format(StaticResources.Get(nameof(Languages.CopyMasterComfirmation), user.Language), credit, username);
        }
    }
}