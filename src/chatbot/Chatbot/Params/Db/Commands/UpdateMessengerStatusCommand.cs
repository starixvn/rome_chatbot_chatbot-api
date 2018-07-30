namespace Chatbot.Params.Db.Commands
{
    public class UpdateMessengerStatusCommand : BaseCommand
    {
        public UpdateMessengerStatusCommand(
            int userId, 
            int messengerType,
            string messengerUsername,
            bool enabled)
        {
            UserId = userId;
            MessengerType = messengerType;
            Enabled = enabled;
            MessengerUsername = messengerUsername;
        }

        public int UserId { get; }

        public int MessengerType { get; }

        public bool Enabled { get; }

        public string MessengerUsername { get; }

        public override string GetSettingKey() => "UpdateMessengerAccountStatus";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0;
    }
}