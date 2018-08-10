namespace Chatbot.WebApi.Params
{
    public class MessageParam
    {
        public int UserId { get; set; }

        public int MessengerType { get; set; }

        public string MessengerUsername { get; set; }

        public string Message { get; set; }

        public int FunctionId { get; set; }
    }
}