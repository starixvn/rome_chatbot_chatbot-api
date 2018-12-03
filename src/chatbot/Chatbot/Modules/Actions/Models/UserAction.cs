namespace Chatbot.Modules.Actions.Models
{
    using System;
    using Newtonsoft.Json;

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

        [JsonProperty]
        public int Id { get; private set; }

        [JsonIgnore]
        public int UserId { get; private set; }

        [JsonProperty]
        public int ActionId { get; private set; }

        [JsonProperty]
        public string UserResponse { get; set; }

        [JsonProperty]
        public DateTime ChatTime { get; private set; }

        [JsonProperty]
        public string CompiledCommand { get; set; }

        public byte Status { get; private set; }

        public bool IsActionValid => Status == ActionValidCode;
    }
}