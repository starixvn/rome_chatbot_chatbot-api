namespace Chatbot.Client.Models
{
    using System;

    public class Message
    {
        public int Id { get; set; }

        public int Sender { get; set; }

        public int Receiver { get; set; }

        public int FunctionId { get; set; }

        public string MessengerUsername { get; set; }

        public int MessengerType { get; set; }

        public string Content { get; set; }

        public DateTime SentDate { get; set; }

        public int Status { get; set; }

        public string SentDateString => SentDate.ToString("MM/dd/yyyy HH:mm:ss");
    }
}