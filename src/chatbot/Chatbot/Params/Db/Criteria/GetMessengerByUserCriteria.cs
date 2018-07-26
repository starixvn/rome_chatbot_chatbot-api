namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessengerByUserCriteria : CriteriaBase
    {
        public GetMessengerByUserCriteria(int userId, int messengerType = 0)
        {
            UserId = userId;
            MessengerType = messengerType;
        }
        public int UserId { get; }

        public int MessengerType { get; }

        public override string GetSettingKey() => "GetMessengerByUserCriteria";

        public override bool IsValid()
            => UserId > 0;
    }
}