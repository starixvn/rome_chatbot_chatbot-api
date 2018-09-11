namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetSettingCriteria : CriteriaBase
    {
        public GetSettingCriteria(string settingName)
        {
            SettingName = settingName;
        }

        public string SettingName { get; }

        public override string GetSettingKey() => "GetSetting";

        public override bool IsValid() => !string.IsNullOrEmpty(SettingName);
    }
}