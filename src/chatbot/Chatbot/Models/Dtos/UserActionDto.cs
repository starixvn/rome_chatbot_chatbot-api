namespace Chatbot.Models.Dtos
{
    using System;

    public class UserActionDto
    {
        public int UserId { get; set; }

        public int MessengerType { get; set; }

        public string MessengerUsername { get; set; }

        public int ActionId { get; set; }

        public int CountTimes { get; set; }

        public DateTime Date { get; set; }
    }
}