namespace Chatbot.WebApi.Params
{
    using System;

    public class MessageLogParam
    {
        public int UserId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int FunctionId { get; set; }

        public int MessengerType { get; set; }
    }
}