namespace Chatbot.Client.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class User
    {
        public int Id { get; set; }

        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public List<Messenger> MessengerAccounts { get; set; }

        public List<Function> Functions { get; set; }

        public string Language { get; set; }

        public bool HasNoMessengerAccount
            => MessengerAccounts == null
            || !MessengerAccounts.Any();
    }
}