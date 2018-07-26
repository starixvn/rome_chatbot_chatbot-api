namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetFunctionNameCriteria : CriteriaBase
    {
        public GetFunctionNameCriteria(int functionId)
        {
            FunctionId = functionId;
        }

        public int FunctionId { get; }

        public override string GetSettingKey() => "GetFunctionNameCriteria";

        public override bool IsValid()
            => FunctionId > 0;
    }
}