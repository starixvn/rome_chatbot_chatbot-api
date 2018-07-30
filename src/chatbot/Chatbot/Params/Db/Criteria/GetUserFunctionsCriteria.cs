namespace Chatbot.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserFunctionsCriteria : CriteriaBase
    {
        public GetUserFunctionsCriteria(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public override string GetSettingKey() => "GetUserFunctions";

        public override bool IsValid() => UserId > 0;
    }
}