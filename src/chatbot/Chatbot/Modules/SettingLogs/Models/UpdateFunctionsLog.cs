namespace Chatbot.Modules.SettingLogs.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;
    using Chatbot.Modules.SettingLogs.Constants;

    public class UpdateFunctionsLog : LogData
    {
        public UpdateFunctionsLog(
            int actorId,
            int custId,
            string ip,
            IEnumerable<Function> newFunctionSettings,
            IEnumerable<Function> oldFunctionSettings)
            : base(actorId, custId, ip)
        {
            NewValues = new Dictionary<string, object>();
            OldValues = new Dictionary<string, object>();

            foreach (var setting in oldFunctionSettings)
            {
                OldValues.Add($"Function{setting.Id}", GetFunctionStatus(setting.Enabled));

                var newSetting = newFunctionSettings.FirstOrDefault(s => s.Id == setting.Id);

                if (newSetting == null)
                {
                    NewValues.Add($"Function{setting.Id}", GetFunctionStatus(setting.Enabled));
                }
                else
                {
                    NewValues.Add($"Function{newSetting.Id}", GetFunctionStatus(newSetting.Enabled));
                }
            }
        }

        public override int ActionId => LogAction.UpdateChatbotFunctions;

        public override string SPName => "Chatbot_UpdateUserFunctions";

        public override IDictionary<string, object> NewValues { get; }

        public override IDictionary<string, object> OldValues { get; }

        private static string GetFunctionStatus(bool isEnabled)
            => isEnabled ? FunctionStatus.Enabled : FunctionStatus.Disabled;
    }
}