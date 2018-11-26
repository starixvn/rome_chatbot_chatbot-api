namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;

    public class DefaultActionNameBuilder : IActionNameBuilder
    {
        private readonly ResourceService resourceService;

        public DefaultActionNameBuilder(ResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        public async Task<string> BuildActionName(int actionId, User user)
            => (await resourceService.GetResource(actionId, ResourceType.Action, user.Language).ConfigureAwait(false)).Value;
    }
}