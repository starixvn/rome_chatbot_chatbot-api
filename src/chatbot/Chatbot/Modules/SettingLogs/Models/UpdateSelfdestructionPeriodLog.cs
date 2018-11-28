namespace Chatbot.Modules.SettingLogs.Models
{
    using System.Collections.Generic;
    using Chatbot.Modules.SettingLogs.Constants;

    public class UpdateSelfdestructionPeriodLog : LogData
    {
        private const string selfdestructionPeriod = "Period";

        public UpdateSelfdestructionPeriodLog(
            int actorId,
            int custId,
            string ip,
            int newValue,
            int? oldValue)
            : base(actorId, custId, ip)
        {
            NewValues = new Dictionary<string, object>
            {
                [selfdestructionPeriod] = SelfdestructionPeriod.Options[newValue]
            };

            if (oldValue.HasValue)
            {
                OldValues = new Dictionary<string, object>
                {
                    [selfdestructionPeriod] = SelfdestructionPeriod.Options[oldValue.Value]
                };
            }
        }

        public override int ActionId => LogAction.UpdateSelfdestructionPeriod;

        public override string SPName => "Chatbot_UpdateSelfdestructionPeriod";

        public override IDictionary<string, object> NewValues { get; }

        public override IDictionary<string, object> OldValues { get; }
    }
}