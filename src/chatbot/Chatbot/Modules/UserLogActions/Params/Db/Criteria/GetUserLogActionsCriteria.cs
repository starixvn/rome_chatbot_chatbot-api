namespace Chatbot.Modules.UserLogActions.Params.Db.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetUserLogActionsCriteria : CriteriaBase
    {
        public GetUserLogActionsCriteria(
            int userId,
            int actionId,
            int messengerType,
            string messengerUsername)
        {
            UserId = userId;
            ActionId = actionId;
            MessengerType = messengerType;
            MessengerUsername = messengerUsername;
            Date = DateTime.Now.Date;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public int MessengerType { get; }

        public string MessengerUsername { get; }

        public DateTime Date { get; }

        public override string GetSettingKey() => "GetUserLogActions";

        public override bool IsValid()
            => UserId > 0
            && ActionId > 0;
    }
}