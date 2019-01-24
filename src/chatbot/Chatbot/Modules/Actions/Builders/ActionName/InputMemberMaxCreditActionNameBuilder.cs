namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Shared.Constants;
    using Chatbot.Shared.Extensions;

    public class InputMemberMaxCreditActionNameBuilder : IActionNameBuilder
    {
        private readonly ResourceService resourceService;

        public InputMemberMaxCreditActionNameBuilder(ResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        public async Task<string> BuildActionName(int actionId, User user, IList<UserAction> userActions = null)
        {
            var resource = await resourceService.GetResource(actionId, ResourceType.Action, user.Language).ConfigureAwait(false);
            var userReponse = userActions.FirstOrDefault(a => a.ActionId == ActionIds.CopyCustomerInputAgentMaxCredit)?.UserResponse;

            return string.Format(resource.Value, (decimal.Parse(userReponse)).ToFormattedNumber());
        }
    }
}