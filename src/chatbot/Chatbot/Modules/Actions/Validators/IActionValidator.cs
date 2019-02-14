namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;

    public interface IActionValidator
    {
        Task<ActionValidationResult> ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger);
    }
}