namespace Chatbot.Params.Db
{
    using System;
    using Fanex.Data.Repository;

    public class InsertVerificationCodeCommand : NonQueryCommand
    {
        public int UserId { get; set; }

        public int MessengerType { get; set; }

        public string VerificationCode { get; set; }

        public DateTime GeneratedTime => DateTime.Now;

        public override string GetSettingKey() => "InsertVerificationCodeCommand";


        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0
            && !string.IsNullOrWhiteSpace(VerificationCode);
    }
}