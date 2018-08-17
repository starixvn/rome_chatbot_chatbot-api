namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class InsertMessageCommand : NonQueryCommand
    {
        public InsertMessageCommand(
            int sender,
            int receiver,
            string message,
            int messengerType,
            string messengerUsername,
            int status,
            int functionId)
        {
            Sender = sender;
            Receiver = receiver;
            FunctionId = functionId;
            MessengerType = messengerType;
            MessengerUsername = messengerUsername;
            Status = status;
            Message = message;
        }

        public int Sender { get; }

        public int Receiver { get; }

        public int FunctionId { get; }

        public int MessengerType { get; }

        public string MessengerUsername { get; }

        public string Message { get; }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public DateTime SentDate => DateTime.Now;
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        public int Status { get; }

        public override string GetSettingKey() => "InsertMessage";

        public override bool IsValid()
            => (Sender != 0 || Receiver != 0)
                && !string.IsNullOrWhiteSpace(Message);
    }
}