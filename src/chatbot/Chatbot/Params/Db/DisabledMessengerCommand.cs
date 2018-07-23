namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class DisabledMessengerCommand : NonQueryCommand
    {
        public int UserId { get; set; }

        public int MessengerType { get; set; }

        public override string GetSettingKey() => "DisabledMessengerCommand";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0;
    }
}