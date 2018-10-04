namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public class UpdateSettingCommand : NonQueryCommand
    {
        public UpdateSettingCommand(
           string settingName,
           string value)
        {
            SettingName = settingName;
            Value = value;
        }

        public string SettingName { get; }

        public string Value { get; }

        public override string GetSettingKey() => "UpdateSetting";

        public override bool IsValid() => !string.IsNullOrEmpty(SettingName);
    }
}