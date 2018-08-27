namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;

    public class ScheduleFunctionHandler : DefaultFunctionHandler
    {
        public ScheduleFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender, CustomerService customerService,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders = null)
            : base(messageBuilder, messageSender, customerService, getDataHanlders)
        {
        }

        /// <summary>
        /// Don't send closed account message for schedule functions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Task<Result> BuildAndSendAccountClosedMessage(FunctionParam param)
            => Task.FromResult(Result.CreateSuccessfulResult("Account is closed"));
    }
}