namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllUsersCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Chatbot_GetAllUsers";

        public override bool IsValid() => true;
    }
}