namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetFailedActionCriteria : CriteriaBase
    {
        public GetFailedActionCriteria(int userId, int actionId)
        {
            UserId = userId;
            ActionId = actionId;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public override string GetSettingKey() => "GetFailedActionCriteria";

        public override bool IsValid()
            => UserId > 0
            && ActionId > 0;
    }
}