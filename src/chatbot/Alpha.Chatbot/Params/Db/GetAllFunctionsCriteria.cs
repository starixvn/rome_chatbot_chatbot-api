namespace Alpha.Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetAllFunctionsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "GetAllFunctionsCriteria";

        public override bool IsValid() => true;
    }
}