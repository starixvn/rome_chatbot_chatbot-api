namespace Chatbot.Modules.Settings.Logs
{
    using System.Collections.Generic;
    using Chatbot.Constants;

    public class UpdateLanguageLog : LogData
    {
        private const string languageKey = "language";

        public UpdateLanguageLog(

            int actorId,
            int custId,
            string ip,
            string newLanguage,
            string oldLanguage)
            : base(actorId, custId, ip)
        {
            NewValues = new Dictionary<string, object>
            {
                [languageKey] = newLanguage
            };

            OldValues = new Dictionary<string, object>
            {
                [languageKey] = oldLanguage
            };
        }

        public override int ActionId => LogAction.UpdateChatbotLanguage;

        public override string SPName => "Chatbot_UpdateUserLanguage";

        public override IDictionary<string, object> NewValues { get; }

        public override IDictionary<string, object> OldValues { get; }
    }
}