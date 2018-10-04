namespace Chatbot.Models.Dtos
{
    using System;

    public class UserActionDto
    {
        public UserActionDto () { }

        public UserActionDto (
            int userId,
            int messengerType,
            string messengerUsername,
            int actionId,
            int countTimes,
            DateTime date)
        {
            UserId = userId;
            MessengerType = messengerType;
            MessengerUsername = messengerUsername;
            ActionId = actionId;
            CountTimes = countTimes;
            Date = date;
        }

        public int UserId { get; set; }

        public int MessengerType { get; set; }

        public string MessengerUsername { get; set; }

        public int ActionId { get; set; }

        public int CountTimes { get; set; }

        public DateTime Date { get; set; }
    }
}