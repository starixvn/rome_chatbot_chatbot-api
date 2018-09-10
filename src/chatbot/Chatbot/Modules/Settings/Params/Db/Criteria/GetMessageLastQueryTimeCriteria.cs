namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessageLastQueryTimeCriteria : CriteriaBase
    {
        public GetMessageLastQueryTimeCriteria(int functionId)
        {
            FunctionId = functionId;
        }

        public int FunctionId { get; }

        public override string GetSettingKey() => "GetMessageLastQueryTime";

        public override bool IsValid() => FunctionId > 0;
    }
}