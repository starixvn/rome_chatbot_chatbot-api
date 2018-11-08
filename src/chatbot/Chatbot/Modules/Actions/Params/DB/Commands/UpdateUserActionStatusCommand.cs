namespace Chatbot.Modules.Actions.Params.DB.Commands
{
    using Fanex.Data.Repository;

    public class UpdateUserActionStatusCommand : NonQueryCommand
    {
        public UpdateUserActionStatusCommand(
            int userActionId,
            bool isCancelOrCompleted)
        {
            UserActionId = userActionId;
            Status = isCancelOrCompleted
                ? 0
                : 1;
        }

        public int UserActionId { get; }

        public int Status { get; set; }

        public override string GetSettingKey()
            => "Chatbot_UpdateUserActionStatus";

        public override bool IsValid()
            => UserActionId > 0;
    }
}