namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Threading.Tasks;
    using Chatbot.Models;

    public interface IActionNameBuilder
    {
        Task<string> BuildActionName(int actionId, User user);
    }
}