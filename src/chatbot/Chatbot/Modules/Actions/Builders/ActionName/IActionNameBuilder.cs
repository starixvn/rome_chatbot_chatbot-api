namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;

    public interface IActionNameBuilder
    {
        Task<string> BuildActionName(int actionId, User user, IList<UserAction> userActions = null);
    }
}