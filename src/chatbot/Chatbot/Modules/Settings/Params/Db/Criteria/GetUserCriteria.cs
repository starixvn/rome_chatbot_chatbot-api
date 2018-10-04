namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserCriteria : CriteriaBase
    {
        public GetUserCriteria(int custId, int subAccountId)
        {
            CustId = custId;
            SubAccountId = subAccountId;
        }

        public GetUserCriteria(int userId)
        {
            UserId = userId;
        }

        public int CustId { get; }

        public int SubAccountId { get; }

        public int UserId { get; }

        public override string GetSettingKey() => "GetUser";

        public override bool IsValid()
            => (CustId > 0 && SubAccountId >= 0)
                || UserId > 0;
    }
}