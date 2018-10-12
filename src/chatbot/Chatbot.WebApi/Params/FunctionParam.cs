namespace Chatbot.WebApi.Params
{
    using System.Collections.Generic;
    using Chatbot.Models;

    public class FunctionParam
    {
        public IEnumerable<Function> Functions { get; set; }

        public LogParam LogParam { get; set; }
    }
}