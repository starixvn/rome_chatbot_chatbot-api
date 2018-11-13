namespace Chatbot.Modules.Actions.Models
{
    using System;

    public class UserAction
    {
        public const int ActionValidCode = 1;

        public UserAction()
        {
        }

        internal UserAction(
            int id,
            int userId,
            int actionId,
            string userResponse,
            DateTime chatTime,
            byte status = ActionValidCode)
        {
            Id = id;
            UserId = userId;
            ActionId = actionId;
            UserResponse = userResponse;
            ChatTime = chatTime;
            Status = status;
        }

        public int Id { get; private set; }

        public int UserId { get; private set; }

        public int ActionId { get; private set; }

        public string UserResponse { get; set; }

        public DateTime ChatTime { get; private set; }

        public string CompiledCommand { get; set; }

        public byte Status { get; private set; }

        public bool IsActionCompletedOrCancel => Status == 0;
    }
}