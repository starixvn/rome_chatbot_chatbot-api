namespace Chatbot.Modules.SettingLogs.Models
{
    using System.Collections.Generic;
    using Chatbot.Constants;
    using Chatbot.Models;
    using Chatbot.Modules.SettingLogs.Constants;

    public class VerifyAccountLog : LogData
    {
        private readonly IDictionary<int, int> verifyAccountLogActions = new Dictionary<int, int>
        {
            [MessengerType.Letstalk] = LogAction.VerifyLetstalkId
        };

        private readonly Messenger messenger;

        public VerifyAccountLog(
            int actorId,
            int custId,
            string ip,
            Messenger messenger)
            : base(actorId, custId, ip)
        {
            this.messenger = messenger;
        }

        public override int ActionId => verifyAccountLogActions[messenger.Type];

        public override string SPName => "Chatbot_UpdateMessengerAccountStatus";

        public override IDictionary<string, object> NewValues
            => new Dictionary<string, object>
            {
                [nameof(messenger.Username)] = messenger.Username
            };
    }
}