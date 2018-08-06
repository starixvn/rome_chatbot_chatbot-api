namespace Chatbot.Models
{
    using System.Collections.Generic;

    public class User
    {
        public User() { }

        public User(int id, int custId, int subAccountId, string language)
        {
            Id = id;
            CustId = custId;
            SubAccountId = subAccountId;
            Language = language;
        }

        public int Id { get; private set; }

        public int CustId { get; private set; }

        public int SubAccountId { get; private set; }

        public IEnumerable<Messenger> MessengerAccounts { get; set; }

        public IEnumerable<Function> Functions { get; set; }

        public string Language { get; private set; }
    }
}