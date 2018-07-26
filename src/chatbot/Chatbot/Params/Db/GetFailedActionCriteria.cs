namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetFailedActionCriteria : CriteriaBase
    {
        public GetFailedActionCriteria(int userId, int actionId, int messengerType)
        {
            UserId = userId;
            ActionId = actionId;
            MessengerType = messengerType;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public int MessengerType { get; }

        public override string GetSettingKey() => "GetFailedActionCriteria";

        public override bool IsValid()
            => UserId > 0
            && ActionId > 0;
    }
}