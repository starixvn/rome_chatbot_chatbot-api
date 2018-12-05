namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Shared.Helpers;

    public class DefaultActionTitleBuilder : IActionTitleBuilder
    {
        public string BuildActionTitle(User user, IList<UserAction> userActions)
            => StaticResources.Get(nameof(Languages.PleaseSelectOption), user.Language);
    }
}