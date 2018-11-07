namespace Chatbot.Modules.Actions.Models
{
    using System;

    public class UserActionItemDto
    {
        public int UserActionId { get; private set; }

        public int UserId { get; private set; }

        public int ActionId { get; private set; }

        public string UserResponse { get; set; }

        public DateTime ChatTime { get; private set; }

        public string CompiledCommand { get; set; }

        public bool Status { get; private set; }
    }
}