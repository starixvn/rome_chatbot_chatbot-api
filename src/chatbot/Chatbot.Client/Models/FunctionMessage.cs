namespace Chatbot.Client.Models
{
    public class FunctionMessage
    {
        public int UserId { get; set; }

        public string Message { get; set; }

        public int? SelfdestructionPeriod { get; set; }
    }
}