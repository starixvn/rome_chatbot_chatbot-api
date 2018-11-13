namespace Chatbot.Models
{
    public class FunctionMessage
    {
        public FunctionMessage(
            int userId,
            string message,
            int? selfdestructionPeriod)
        {
            UserId = userId;
            Message = message;
            SelfdestructionPeriod = selfdestructionPeriod;
        }

        public int UserId { get; }

        public string Message { get; }

        public int? SelfdestructionPeriod { get; }
    }
}