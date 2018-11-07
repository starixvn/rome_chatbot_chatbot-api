namespace Chatbot.Modules.Actions.Validators
{
    using System;
    using Chatbot.Models;

    public interface IActionValidator
    {
        Result ValidateAction(int actionId, User user, string userResponse);
    }

    public class NumberValueActionValidator : IActionValidator
    {
        public Result ValidateAction(int actionId, User user, string userResponse)
        {
            throw new NotImplementedException();
        }
    }

    public class UsernameActionValidator : IActionValidator
    {
        public Result ValidateAction(int actionId, User user, string userResponse)
        {
            throw new NotImplementedException();
        }
    }
}