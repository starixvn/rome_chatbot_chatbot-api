namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessengerCriteria : CriteriaBase
    {
        public GetMessengerCriteria(string messengerAccount, int messengerType)
        {
            MessengerAccount = messengerAccount;
            MessengerType = messengerType;
        }

        public string MessengerAccount { get; }

        public int MessengerType { get; }

        public override string GetSettingKey()
            => "GetMessengerCriteria";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MessengerAccount)
            && MessengerType > 0;
    }
}