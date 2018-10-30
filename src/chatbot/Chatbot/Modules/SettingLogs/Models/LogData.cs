namespace Chatbot.Modules.SettingLogs.Models
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Models;

    public abstract class LogData : ILogData
    {
        protected LogData(
            int actorId,
            int custId,
            string ip)
        {
            ActorId = actorId;
            CustId = custId;
            IP = ip;
        }

        public int ActorId { get; }

        public int CustId { get; }

        public abstract int ActionId { get; }

        public abstract string SPName { get; }

        public string IP { get; }

        public abstract IDictionary<string, object> NewValues { get; }

        public virtual IDictionary<string, object> OldValues { get; }
    }
}