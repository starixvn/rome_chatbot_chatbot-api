namespace Chatbot.Modules.Actions.Params.DB.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetActionsByUser : CriteriaBase
    {
        public GetActionsByUser(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public override string GetSettingKey()
            => "GetActionsByUser";

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}