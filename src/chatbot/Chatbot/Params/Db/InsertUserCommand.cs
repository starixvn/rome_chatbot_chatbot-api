namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class InsertUserCommand : NonQueryCommand
    {
        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public string Language { get; set; }

        public override string GetSettingKey() => "InsertUserCommand";

        public int UserId { get; set; }

        public override bool IsValid()
            => CustId > 0
            && SubAccountId > 0
            && !string.IsNullOrWhiteSpace(Language);
    }
}