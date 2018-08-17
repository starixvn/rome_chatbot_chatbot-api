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
            IDictionary<string, object> others)
        {
            User = user;
            Function = function;
            Messenger = messenger;
            Others = others;
        }

        public User User { get; }

        public Function Function { get; }

        public Messenger Messenger { get; }

        public IDictionary<string, object> Others { get; }
    }
}