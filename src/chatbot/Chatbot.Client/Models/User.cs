namespace Chatbot.Client.Models
{
    using System.Linq;
    using System.Collections.Generic;

    public class User
    {
        public int Id { get; set; }

        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public IEnumerable<Messenger>  MessengerAccounts { get; set; }

        public IEnumerable<Function> Functions { get; set; }

        public string Language { get; set; }

        public bool HasNoMessengerAccount
            => MessengerAccounts == null
            || !MessengerAccounts.Any();
    }
}