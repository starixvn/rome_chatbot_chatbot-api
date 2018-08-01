namespace Chatbot.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public class UpdateMessengerStatusCommand : NonQueryCommand
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
            && MessengerType > 0
            && !string.IsNullOrWhiteSpace(MessengerUsername);
    }
}