namespace Chatbot.Models
{
    using System;

    public class Message
    {
        public Message()
        {
        }

        internal Message(
            int id,
            int sender,
            int receiver,
            int functionId,
            string messengerUsername,
            int messengerType,
            string content,
            DateTime sentDate,
            int status)
        {
            Id = id;
            Sender = sender;
            Receiver = receiver;
            FunctionId = functionId;
            MessengerUsername = messengerUsername;
            MessengerType = messengerType;
            Content = content;
            SentDate = sentDate;
            Status = status;
        }

        public int Id { get; private set; }

        public int Sender { get; private set; }

        public int Receiver { get; private set; }

        public int FunctionId { get; private set; }

        public string MessengerUsername { get; private set; }

        public int MessengerType { get; private set; }

        public string Content { get; private set; }

        public DateTime SentDate { get; private set; }

        public int Status { get; private set; }
    }
}