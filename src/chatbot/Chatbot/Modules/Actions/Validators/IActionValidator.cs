namespace Chatbot.Modules.Actions.Validators
{
    using Chatbot.Models;

    public interface IActionValidator
    {
        Result ValidateAction(int actionId, User user, string userResponse);
    }
}