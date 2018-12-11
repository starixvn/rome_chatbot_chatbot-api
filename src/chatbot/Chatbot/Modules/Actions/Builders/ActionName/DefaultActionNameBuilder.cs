namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Shared.Extensions;

    public class DefaultActionNameBuilder : IActionNameBuilder
    {
        private readonly ResourceService resourceService;
        private const string ActionIdPattern = "#actionid_";
        private static readonly Regex ActionIdRegex = new Regex($@"(?<={ActionIdPattern})\w+");

        public DefaultActionNameBuilder(ResourceService resourceService)
        {
            this.resourceService = resourceService;
        }

        public async Task<string> BuildActionName(int actionId, User user, IList<UserAction> userActions = null)
        {
            var resourceValue = (await resourceService.GetResource(actionId, ResourceType.Action, user.Language).ConfigureAwait(false)).Value;

            return BuildName(userActions, resourceValue);
        }

        private static string BuildName(IList<UserAction> userActions, string resourceValue)
        {
            var actionIdMatches = ActionIdRegex.Matches(resourceValue);

            if (actionIdMatches.Any() && userActions != null)
            {
                foreach (Match match in actionIdMatches)
                {
                    var preActionId = int.Parse(match.Value);
                    var preUserAction = userActions.FirstOrDefault(userAction => userAction.ActionId == preActionId);

                    if (preUserAction != null)
                    {
                        resourceValue = resourceValue.Replace(
                            ActionIdPattern + match.Value,
                            FormatUserReponse(preUserAction.UserResponse));
                    }
                }
            }

            return resourceValue;
        }

        private static string FormatUserReponse(string text)
        {
            var isNumeric = decimal.TryParse(text, out decimal number);

            return isNumeric
                ? number.ToFormattedNumber()
                : text;
        }
    }
}