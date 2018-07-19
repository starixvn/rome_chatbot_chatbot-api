namespace Alpha.Chatbot.Models.Dtos
{
    public class UserDto
    {
        public int UserId { get; set; }

        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public int MessengerType { get; set; }

        public string MessengerAcc { get; set; }

        public bool Enabled { get; set; }

        public string Language { get; set; }
    }
}