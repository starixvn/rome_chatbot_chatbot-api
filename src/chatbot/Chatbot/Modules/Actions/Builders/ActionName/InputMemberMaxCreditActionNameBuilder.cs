namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Extensions;

    public class InputMemberMaxCreditActionNameBuilder : IActionNameBuilder
    {
        private readonly ResourceService resourceService;
        private readonly IChatbotConfigs chatbotConfigs;

        public InputMemberMaxCreditActionNameBuilder(
            ResourceService resourceService,
            IChatbotConfigs chatbotConfigs)
        {
            this.resourceService = resourceService;
            this.chatbotConfigs = chatbotConfigs;
        }

        public async Task<string> BuildActionName(int actionId, User user, IList<UserAction> userActions = null)
        {
            var resource = await resourceService.GetResource(actionId, ResourceType.Action, user.Language).ConfigureAwait(false);
            var userReponse = userActions.FirstOrDefault(a => a.ActionId == chatbotConfigs.CopyAgentInputCreditActionId)?.UserResponse;

            return string.Format(resource.Value, (decimal.Parse(userReponse)).ToFormattedNumber());
        }
    }
}