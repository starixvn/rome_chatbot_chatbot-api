namespace Chatbot.Params.Db.Commands
{
    public class InsertUserCommand : BaseCommand
    {
        public InsertUserCommand(int custId, int subAccountId, string language)
        {
            CustId = custId;
            SubAccountId = subAccountId;
            Language = language;
        }

        public int CustId { get; }

        public int SubAccountId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "InsertUser";

        public int UserId { get; set; }

        public override bool IsValid()
            => CustId > 0
            && SubAccountId >= 0
            && !string.IsNullOrWhiteSpace(Language);
    }
}