namespace Alpha.Chatbot.Models
{
    using System.Collections.Generic;

    public class User
    {
        public int Id { get; set; }

        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public IEnumerable<MessengerAccount>  MessengerAccounts { get; set; }

        public bool Enabled { get; set; }

        public string Language { get; set; }
    }
}