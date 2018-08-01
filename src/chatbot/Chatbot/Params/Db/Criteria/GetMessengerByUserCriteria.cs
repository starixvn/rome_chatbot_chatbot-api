namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessengerByUserCriteria : CriteriaBase
    {
        public GetMessengerByUserCriteria(
            int userId, 
            int messengerType = 0,
            string messengerUsername = null)
        {
            UserId = userId;
            MessengerType = messengerType;
            MessengerUsername = messengerUsername;
        }
        public int UserId { get; }

        public int MessengerType { get; }

        public string MessengerUsername { get; }

        public override string GetSettingKey() => "GetMessengerByUser";

        public override bool IsValid()
            => UserId > 0;
    }
}