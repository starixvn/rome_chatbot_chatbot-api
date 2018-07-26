namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetFunctionsCriteria : CriteriaBase
    {
        public GetFunctionsCriteria(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public override string GetSettingKey() => "GetFunctionsCriteria";

        public override bool IsValid() => UserId > 0;
    }
}