namespace Chatbot.Modules.Funtions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Helpers;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;

    public class MessageAnnouncementFunctionHandler : DefaultFunctionHandler
    {
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;

        public MessageAnnouncementFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null) :
                base(messageBuilder, messageSender, customerService, getDataHandlers)
        {
            this.getDataHandlers = getDataHandlers;
            SendMessageInCaseAccountClosed = false;
        }

        public override async Task<Result> Handle(FunctionParam param)
        {
            // Check Params
            var results = new List<Result>();

            if (!getDataHandlers.ContainsKey(param.Function.Id))
            {
                throw new InvalidOperationException($"Cannot find get data function for function: {param.Function.Id}-{param.Function.Name}");
            }

            if (await IsCustomerClosed(param.User).ConfigureAwait(false))
            {
                return Result.CreateSuccessfulResult("Customer is closed");
            }

            var data = await getDataHandlers[param.Function.Id](param).ConfigureAwait(false);
            var messages = DataHelper.TryParse<IList<Alpha.Services.Client.Models.Message>>(data);

            if (messages == null || messages.Count == 0)
            {
                return Result.CreateSuccessfulResult("No data");
            }

            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(message.Content))
                {
                    results.Add(await BuildAndSendMessage(param, message).ConfigureAwait(false));
                }
            }

            return results.All(result => result.IsOk)
                ? Result.CreateSuccessfulResult()
                : Result.CreateErrorResult();
        }
    }
}