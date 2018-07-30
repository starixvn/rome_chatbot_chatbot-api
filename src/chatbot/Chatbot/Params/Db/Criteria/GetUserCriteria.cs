namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserCriteria : CriteriaBase
    {
        public GetUserCriteria(int custId, int subAccountId)
        {
            CustId = custId;
            SubAccountId = subAccountId;
        }

        public int CustId { get; }

        public int SubAccountId { get; }

        public override string GetSettingKey() => "GetUser";

        public override bool IsValid() 
            => CustId > 0
            && SubAccountId >= 0;
    }
}