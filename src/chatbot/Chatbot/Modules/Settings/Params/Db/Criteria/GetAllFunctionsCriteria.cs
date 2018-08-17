namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllFunctionsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "GetAllFunctionsCriteria";

        public override bool IsValid() => true;
    }
}