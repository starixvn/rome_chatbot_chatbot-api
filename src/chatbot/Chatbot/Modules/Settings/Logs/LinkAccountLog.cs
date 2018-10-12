namespace Chatbot.Modules.Settings.Logs
{
    using System.Collections.Generic;
    using Chatbot.Constants;
    using Chatbot.Models;

    public class LinkAccountLog : LogData
    {
        private readonly IDictionary<int, int> linkAccountLogActions = new Dictionary<int, int>
        {
            [MessengerType.Letstalk] = LogAction.LinkLetstalkId
        };

        private readonly Messenger messenger;

        public LinkAccountLog(
            int actorId,
            int custId,
            string ip,
            Messenger messenger)
            : base(actorId, custId, ip)
        {
            this.messenger = messenger;
        }

        public override int ActionId => linkAccountLogActions[messenger.Type];

        public override string SPName => "Chatbot_InsertUser";

        public override IDictionary<string, object> NewValues
            => new Dictionary<string, object>
            {
                [nameof(messenger.Username)] = messenger.Username
            };
    }
}