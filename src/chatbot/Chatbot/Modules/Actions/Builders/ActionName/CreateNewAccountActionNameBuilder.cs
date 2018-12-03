namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Modules.Resources.Models;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Extensions;

    public class CreateNewAccountActionNameBuilder : IActionNameBuilder
    {
        private readonly ResourceService resourceService;
        private readonly IChatbotConfigs chatbotConfigs;

        public CreateNewAccountActionNameBuilder(
            ResourceService resourceService,
            IChatbotConfigs chatbotConfigs)
        {
            this.resourceService = resourceService;
            this.chatbotConfigs = chatbotConfigs;
        }

        public async Task<string> BuildActionName(
            int actionId,
            User user,
            IList<UserAction> userActions = null)
        {
            var resource = await resourceService.GetResource(actionId, ResourceType.Action, user.Language).ConfigureAwait(false);

            switch (user.CustLevel)
            {
                case CustLevel.AgentLevel:
                    return BuildCloneMasterOrMemberActionName(
                         resource,
                         GetUserResponse(userActions, chatbotConfigs.CopyMemberInputUsernameActionId),
                         FormatNumber(GetUserResponse(userActions, chatbotConfigs.CopyMemberInputCreditActionId)));

                case CustLevel.MasterLevel:
                    return BuildCloneAgentActionName(
                        resource,
                        GetUserResponse(userActions, chatbotConfigs.CopyAgentInputUsernameActionId),
                        FormatNumber(GetUserResponse(userActions, chatbotConfigs.CopyAgentInputCreditActionId)),
                        FormatNumber(GetUserResponse(userActions, chatbotConfigs.CopyAgentInputMemberCreditActionId)));

                default:
                    return BuildCloneMasterOrMemberActionName(
                        resource,
                        GetUserResponse(userActions, chatbotConfigs.CopyMasterInputUsernameActionId),
                        FormatNumber(GetUserResponse(userActions, chatbotConfigs.CopyMasterInputCreditActionId)));
            }
        }

        private static string FormatNumber(string value)
            => decimal.Parse(value).ToFormattedNumber();

        private static string GetUserResponse(IList<UserAction> userActions, int actionId)
            => userActions
                    .FirstOrDefault(a => a.ActionId == actionId)
                    ?.UserResponse;

        private static string BuildCloneMasterOrMemberActionName(Resource resource, string username, string credit)
            => string.Format(
                resource.Value,
                username,
                decimal.Parse(credit).ToFormattedNumber());

        private static string BuildCloneAgentActionName(Resource resource, string username, string credit, string maxMemberCredit)
            => string.Format(
                resource.Value,
                username,
                decimal.Parse(credit).ToFormattedNumber(),
                decimal.Parse(maxMemberCredit).ToFormattedNumber());
    }
}