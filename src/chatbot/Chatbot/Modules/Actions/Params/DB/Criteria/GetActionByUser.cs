namespace Chatbot.Modules.Actions.Params.DB.Criteria
{
    using Fanex.Data.Repository;

    public class GetActionsByUser : CriteriaBase
    {
        public GetActionsByUser(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public override string GetSettingKey()
            => "Chatbot_GetUserActionsInChain";

        public override bool IsValid()
            => UserId > 0;
    }
}