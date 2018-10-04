namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessengerCriteria : CriteriaBase
    {
        public GetMessengerCriteria(
            string messengerUsername,
            int messengerType,
            bool enabled = true)
        {
            MessengerUsername = messengerUsername;
            MessengerType = messengerType;
            Status = enabled ? 1 : 0;
        }

        public string MessengerUsername { get; }

        public int MessengerType { get; }

        public int Status { get; }

        public override string GetSettingKey()
            => "GetMessenger";

        public override bool IsValid()
            => !string.IsNullOrWhiteSpace(MessengerUsername)
            && MessengerType > 0;
    }
}