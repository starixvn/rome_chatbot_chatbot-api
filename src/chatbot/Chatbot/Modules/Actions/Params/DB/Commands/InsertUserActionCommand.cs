namespace Chatbot.Modules.Actions.Params.DB.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class InsertUserActionCommand : NonQueryCommand
    {
        public InsertUserActionCommand(
            int userId,
            int actionId,
            string userReponse,
            bool isBeginAction,
            string compileCommand)
        {
            UserId = userId;
            ActionId = actionId;
            UserResponse = userReponse;
            IsBeginAction = isBeginAction;
            CompiledCommand = compileCommand;
        }

        public int UserId { get; }

        public int ActionId { get; }

        public string UserResponse { get; }

        public DateTime ChatTime => DateTime.Now;

        public bool IsBeginAction { get; }

        public string CompiledCommand { get; }

        public override string GetSettingKey()
            => "Setting_Key";

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}