namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public class UpdateSelfdestructionPeriodCommand : NonQueryCommand
    {
        public UpdateSelfdestructionPeriodCommand(int userId, int minutes)
        {
            UserId = userId;
            Minutes = minutes;
        }

        public int UserId { get; }

        public int Minutes { get; }

        public override string GetSettingKey()
            => "Chatbot_UpdateSelfdestructionPeriod";

        public override bool IsValid()
            => UserId > 0 && Minutes > 0;
    }
}