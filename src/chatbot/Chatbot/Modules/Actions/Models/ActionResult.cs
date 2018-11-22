namespace Chatbot.Modules.Actions.Models
{
    using Messenger.Client.Models;

    public class ActionResult
    {
        public ActionResult(
            Message message,
            string validationFailedMessage,
            string command,
            int functionId)
        {
            Message = message;
            Command = command;
            FunctionId = functionId;
            ValidationFailedMessage = validationFailedMessage;
        }

        public ActionResult(Message message, string validationFailedMessage, int functionId)
            : this(message, validationFailedMessage, string.Empty, functionId)
        {
        }

        public ActionResult(Message message, int functionId)
            : this(message, string.Empty, string.Empty, functionId)
        {
        }

        public ActionResult(string command, int functionId)
            : this(null, string.Empty, command, functionId)
        {
        }

        public Message Message { get; }

        public string ValidationFailedMessage { get; }

        public string Command { get; }

        public int FunctionId { get; }
    }
}