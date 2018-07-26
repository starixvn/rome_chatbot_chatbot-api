namespace Chatbot.Params.Db
{
    using System;
    using Fanex.Data.Repository;

    public class InsertVerificationCodeCommand : NonQueryCommand
    {
        public InsertVerificationCodeCommand(int userId, int messengerType, string verificationCode)
        {
            UserId = userId;
            MessengerType = messengerType;
            VerificationCode = verificationCode;
        }

        public int UserId { get; }

        public int MessengerType { get; }

        public string VerificationCode { get; }

        public DateTime GeneratedTime => DateTime.Now;

        public override string GetSettingKey() => "InsertVerificationCodeCommand";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0
            && !string.IsNullOrWhiteSpace(VerificationCode);
    }
}