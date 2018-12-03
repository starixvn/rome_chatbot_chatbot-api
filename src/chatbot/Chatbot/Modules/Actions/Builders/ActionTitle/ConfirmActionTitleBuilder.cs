namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Shared.Helpers;

    public class ConfirmActionTitleBuilder : IActionTitleBuilder
    {
        public string BuildActionTitle(User user, IList<UserAction> userActions)
            => StaticResources.Get(nameof(Languages.ActionConfirmation), user.Language);
    }
}
