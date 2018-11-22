namespace Chatbot.Modules.Actions.Params.DB.Commands
{
    using Fanex.Data.Repository;

    public class ResetUserActionCommand : NonQueryCommand
    {
        public ResetUserActionCommand(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; }

        public override string GetSettingKey()
            => "Chatbot_ResetUserActionStatus";

        public override bool IsValid()
            => UserId > 0;
    }
}