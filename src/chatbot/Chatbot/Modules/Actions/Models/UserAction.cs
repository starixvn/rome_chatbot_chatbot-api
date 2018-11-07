namespace Chatbot.Modules.Actions.Models
{
    using System;

    public class UserAction
    {
        public UserAction(
            int id,
            int userId,
            int actionId,
            string userResponse,
            DateTime chatTime)
        {
            Id = id;
            UserId = userId;
            ActionId = actionId;
            UserResponse = userResponse;
            ChatTime = chatTime;
        }

        public int Id { get; private set; }

        public int UserId { get; private set; }

        public int ActionId { get; private set; }

        public string UserResponse { get; set; }

        public DateTime ChatTime { get; private set; }

        public string CompiledCommand { get; set; }

        public bool Status { get; private set; }
    }
}