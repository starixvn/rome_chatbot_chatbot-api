namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllFunctionsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Chatbot_GetAllFunctions";

        public override bool IsValid() => true;
    }
}