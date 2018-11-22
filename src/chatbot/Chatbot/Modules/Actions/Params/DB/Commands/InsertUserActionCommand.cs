namespace Chatbot.Modules.Actions.Params.DB.Commands
{
    using Fanex.Data.Repository;

    public class InsertUserActionCommand : NonQueryCommand
    {
        public InsertUserActionCommand(
            int userId,
            int actionId,
            string userReponse,
            string compileCommand)
        {
            UserId = userId;
            ActionId = actionId;
            UserResponse = userReponse;
            CompiledCommand = compileCommand;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public string UserResponse { get; }

        public string CompiledCommand { get; }

        public override string GetSettingKey()
            => "Chatbot_InsertUserAction";

        public override bool IsValid()
            => UserId > 0 && ActionId > 0;
    }
}