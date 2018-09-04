namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public class InsertUserCommand : NonQueryCommand
    {
        public InsertUserCommand(
            int custId, 
            int subAccountId, 
            string language,
            int custLevel)
        {
            CustId = custId;
            SubAccountId = subAccountId;
            Language = language;
            CustLevel = custLevel;
        }

        public int CustId { get; }

        public int SubAccountId { get; }

        public int CustLevel { get; }

        public string Language { get; }

        public override string GetSettingKey() => "InsertUser";

        public int UserId { get; set; }

        public override bool IsValid()
            => CustId > 0
            && SubAccountId >= 0
            && !string.IsNullOrWhiteSpace(Language)
            && CustLevel > 0;
    }
}