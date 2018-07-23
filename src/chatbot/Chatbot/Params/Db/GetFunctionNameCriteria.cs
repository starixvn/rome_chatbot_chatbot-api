namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetFunctionNameCriteria : CriteriaBase
    {
        public int FunctionId { get; set; }

        public override string GetSettingKey() => "GetFunctionNameCriteria";

        public override bool IsValid()
            => FunctionId > 0;
    }
}