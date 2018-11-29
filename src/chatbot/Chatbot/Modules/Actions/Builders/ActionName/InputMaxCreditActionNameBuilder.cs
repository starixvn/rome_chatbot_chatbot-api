namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Shared.Extensions;

    public class InputMaxCreditActionNameBuilder : IActionNameBuilder
    {
        private readonly ResourceService resourceService;
        private readonly CopyCustomerService copyCustomerService;

        public InputMaxCreditActionNameBuilder(
            ResourceService resourceService,
            CopyCustomerService copyCustomerService)
        {
            this.resourceService = resourceService;
            this.copyCustomerService = copyCustomerService;
        }

        public async Task<string> BuildActionName(int actionId, User user)
        {
            var pt = await copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel).ConfigureAwait(false);
            var resource = await resourceService.GetResource(actionId, ResourceType.Action, user.Language).ConfigureAwait(false);

            return string.Format(resource.Value, pt.MaxCredit.ToFormattedNumber());
        }
    }
}