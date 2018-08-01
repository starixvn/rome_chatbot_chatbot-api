namespace Chatbot.Params.Db.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InsertMessageCommand : BaseCommand
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

        public DateTime SentDate => DateTime.Now;

        public int Status { get; }

        public override string GetSettingKey() => "InsertMessage";

        public override bool IsValid()
            => (Sender > 0 || Receiver > 0)
                && !string.IsNullOrWhiteSpace(Message);
    }
}
