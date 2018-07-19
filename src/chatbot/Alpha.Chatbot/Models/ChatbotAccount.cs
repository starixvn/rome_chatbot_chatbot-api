namespace Alpha.Chatbot.Models
{
    public class ChatbotAccount
    {
        public int Id { get; set; }

        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public MessengerAccount MessengerAccount { get; set; }

        public bool Enabled { get; set; }

        public string Language { get; set; }
    }
}