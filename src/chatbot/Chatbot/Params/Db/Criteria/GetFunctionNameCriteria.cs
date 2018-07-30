namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetFunctionNameCriteria : CriteriaBase
    {
        public GetFunctionNameCriteria(int functionId)
        {
            FunctionId = functionId;
        }

        public int FunctionId { get; }

        public override string GetSettingKey() => "GetFunctionName";

        public override bool IsValid()
            => FunctionId > 0;
    }
}