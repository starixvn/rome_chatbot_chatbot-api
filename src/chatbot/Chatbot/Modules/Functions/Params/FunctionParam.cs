namespace Chatbot.Modules.Funtions.Params
{
    using System.Collections.Generic;
    using Chatbot.Models;

    public class FunctionParam
    {
        public FunctionParam(
            User user,
            Function function,
            Messenger messenger,
            IDictionary<string, object> others,
            IEnumerable<User> users = null)
        {
            User = user;
            Function = function;
            Messenger = messenger;
            Others = others;
            Users = users;
        }

        public User User { get; }

        public Function Function { get; }

        public Messenger Messenger { get; }

        public IDictionary<string, object> Others { get; }

        public IEnumerable<User> Users { get; }
    }
}