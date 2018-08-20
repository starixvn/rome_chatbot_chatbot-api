namespace Chatbot.Models
{
    using System.Collections.Generic;

    public class User
    {
        public User() { }

        internal User(
            int id, 
            int custId, 
            int subAccountId, 
            string language,
            IEnumerable<Messenger> messengerAccounts,
            IEnumerable<Function> functions)
        {
            Id = id;
            CustId = custId;
            SubAccountId = subAccountId;
            Language = language;
            MessengerAccounts = messengerAccounts;
            Functions = functions;
        }

        public int Id { get; private set; }

        public int CustId { get; private set; }

        public int SubAccountId { get; private set; }

        public IEnumerable<Messenger> MessengerAccounts { get; private set; }

        public IEnumerable<Function> Functions { get; private set; }

        public string Language { get; private set; }
    }
}