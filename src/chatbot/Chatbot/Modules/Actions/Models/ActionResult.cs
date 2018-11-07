namespace Chatbot.Modules.Actions.Models
{
    using Messenger.Client.Models;

    public class ActionResult
    {
        public ActionResult(
            Message message,
            string command,
            int functionId)
        {
            Message = message;
            Command = command;
            FunctionId = functionId;
        }

        public ActionResult(Message message, int functionId)
            : this(message, string.Empty, functionId)
        {
        }

        public ActionResult(string command, int functionId)
            : this(null, command, functionId)
        {
        }

        public Message Message { get; }

        public string Command { get; }

        public int FunctionId { get; }
    }
}