namespace Chatbot.Params.Db.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class InsertMessengerCommand : NonQueryCommand
    {
        public InsertMessengerCommand(
            int userId, 
            int messengerType, 
            string messengerUsername, 
            string verificationCode)
        {
            UserId = userId;
            MessengerType = messengerType;
            VerificationCode = verificationCode;
            MessengerUsername = messengerUsername;
        }

        public int UserId { get; }

        public int MessengerType { get; }

        public string VerificationCode { get; }

        public string MessengerUsername { get; }

        public DateTime GeneratedTime => DateTime.Now;

        public override string GetSettingKey() => "InsertMessengerAccount";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0
            && !string.IsNullOrWhiteSpace(VerificationCode)
            && !string.IsNullOrWhiteSpace(MessengerUsername);
    }
}