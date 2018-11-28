[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Chatbot.WebApi.Tests")]

namespace Chatbot.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class User
    {
        public User()
        {
        }

        internal User(
            int id,
            int custId,
            int custLevel,
            int subAccountId,
            string language,
            int? selfdestructionPeriod)
            : this(
                  id,
                  custId,
                  custLevel,
                  subAccountId,
                  language,
                  selfdestructionPeriod,
                  Enumerable.Empty<Messenger>(),
                  Enumerable.Empty<Function>())
        {
        }

        internal User(
            int id,
            int custId,
            int custLevel,
            int subAccountId,
            string language,
            int? selfdestructionPeriod,
            IEnumerable<Messenger> messengerAccounts,
            IEnumerable<Function> functions)
        {
            Id = id;
            CustId = custId;
            SubAccountId = subAccountId;
            Language = language;
            MessengerAccounts = messengerAccounts;
            Functions = functions;
            CustLevel = custLevel;
            SelfdestructionPeriod = selfdestructionPeriod;
        }

        public int Id { get; private set; }

        public int CustId { get; private set; }

        public int SubAccountId { get; private set; }

        public IEnumerable<Messenger> MessengerAccounts { get; private set; }

        public IEnumerable<Function> Functions { get; internal set; }

        public string Language { get; private set; }

        public int CustLevel { get; }

        public int? SelfdestructionPeriod { get; }
    }
}