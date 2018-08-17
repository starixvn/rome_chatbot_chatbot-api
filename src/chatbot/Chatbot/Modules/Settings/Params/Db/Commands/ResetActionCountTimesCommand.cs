namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class ResetActionCountTimesCommand : NonQueryCommand
    {
        public ResetActionCountTimesCommand(
            int userId,
            int actionId,
            int messengerType,
            string messengerUsername)
        {
            UserId = userId;
            ActionId = actionId;
            MessengerType = messengerType;
            MessengerUsername = messengerUsername;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public int MessengerType { get; }

        public string MessengerUsername { get; }

        public DateTime Date => DateTime.Now.Date;

        public override string GetSettingKey() => "ResetUserActionCount";

        public override bool IsValid()
            => UserId > 0
            && ActionId > 0
            && MessengerType > 0
            && !string.IsNullOrWhiteSpace(MessengerUsername);
    }
}