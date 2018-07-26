namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class DisabledMessengerCommand : NonQueryCommand
    {
        public DisabledMessengerCommand(int userId, int messengerType)
        {
            UserId = userId;
            MessengerType = messengerType;
        }

        public int UserId { get; }

        public int MessengerType { get; }

        public override string GetSettingKey() => "DisabledMessengerCommand";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0;
    }
}