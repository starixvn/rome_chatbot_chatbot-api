namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetUsersByFunctionCriteria : CriteriaBase
    {
        public GetUsersByFunctionCriteria(
            int functionId,
            int environment)
        {
            FunctionId = functionId;
            Environment = environment;
        }

        public int FunctionId { get; }

        public int Environment { get; }

        public override string GetSettingKey() => "Chatbot_GetUsersByFunctions";

        public override bool IsValid() => FunctionId > 0 && Environment >= 0;
    }
}