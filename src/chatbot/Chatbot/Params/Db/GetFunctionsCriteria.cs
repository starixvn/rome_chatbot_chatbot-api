namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetFunctionsCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "GetFunctionsCriteria";

        public override bool IsValid() => UserId > 0;
    }
}