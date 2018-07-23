namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetUserCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public override string GetSettingKey() => "GetUser";

        public override bool IsValid() => CustId > 0;
    }
}