namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessengerCriteria : CriteriaBase
    {
        public GetMessengerCriteria(string messengerUsername, int messengerType)
        {
            MessengerUsername = messengerUsername;
            MessengerType = messengerType;
        }

        public string MessengerUsername { get; }

        public int MessengerType { get; }

        public override string GetSettingKey()
            => "GetMessenger";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MessengerUsername)
            && MessengerType > 0;
    }
}