namespace Chatbot.Modules.Actions.Params.DB.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateUserActionCommand : NonQueryCommand
    {
        public UpdateUserActionCommand(
            int userActionId,
            string userReponse,
            string compiledCommand)
        {
            UserActionId = userActionId;
            UserResponse = userReponse;
            CompiledCommand = compiledCommand;
        }

        public int UserActionId { get; }

        public string UserResponse { get; }

        public string CompiledCommand { get; }

        public override string GetSettingKey()
            => "UpdateUserActionCommand";

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}