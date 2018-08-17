namespace Chatbot.Models
{
    using System.Collections.Generic;

    public class User
    {
        public User(
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

        public int Id { get; }

        public int CustId { get; }

        public int SubAccountId { get; }

        public IEnumerable<Messenger> MessengerAccounts { get; }

        public IEnumerable<Function> Functions { get; }

        public string Language { get; }
    }
}