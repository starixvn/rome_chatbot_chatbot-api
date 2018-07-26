namespace Chatbot.Params.Db.Commands
{
    public class UpdateFailActionCommand : BaseCommand
    {
        public UpdateFailActionCommand(int userId, int actionId, int messengerType)
        {
            UserId = userId;
            ActionId = actionId;
            MessengerType = messengerType;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public int MessengerType { get; }

        public override string GetSettingKey() => "UpdateFailActionCommand";

        public override bool IsValid()
            => UserId > 0
            && ActionId > 0
            && MessengerType > 0;
    }
}