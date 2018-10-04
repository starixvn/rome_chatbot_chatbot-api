namespace Chatbot.Models
{
    public class FunctionMessage
    {
        public FunctionMessage(int userId, string message)
        {
            UserId = userId;
            Message = message;
        }

        public int UserId { get; }

        public string Message { get; }
    }
}