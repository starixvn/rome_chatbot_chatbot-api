namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;

    public interface IActionValidator
    {
        Result ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger);
    }
}