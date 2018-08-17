namespace Chatbot.Modules.Funtions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Chatbot.Helpers;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;

    public class MessageAnnouncementFunctionHandler : DefaultFunctionHandler
    {
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders;

        public MessageAnnouncementFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders = null) :
                base(messageBuilder, messageSender, getDataHanlders)
        {
            this.getDataHanlders = getDataHanlders;
        }

        public override async Task<Result> Handle(FunctionParam param)
        {
            // Check Params
            var results = new List<Result>();

            if (!getDataHanlders.ContainsKey(param.Function.Id))
            {
                throw new InvalidOperationException($"Cannot find get data function for function: {param.Function.Id}-{param.Function.Name}");
            }

            var data = await getDataHanlders[param.Function.Id](param);
            var messages = DataHelper.TryParse<IList<Message>>(data);

            if (messages == null || messages.Count == 0)
            {
                return new Result(0, "No data");
            }

            foreach (var message in messages)
            {
                results.Add(await BuildAndSendMessage(param, message));
            }

            return results.All(result => result.IsOk)
                ? Result.CreateSuccessfulResult()
                : new Result(1);
        }
    }
}