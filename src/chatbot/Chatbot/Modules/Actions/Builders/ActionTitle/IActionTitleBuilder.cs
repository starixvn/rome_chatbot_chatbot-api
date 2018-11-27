namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;

    public interface IActionTitleBuilder
    {
        string BuildActionTitle(int actionId, User user, IList<UserAction> userActions);
    }
}