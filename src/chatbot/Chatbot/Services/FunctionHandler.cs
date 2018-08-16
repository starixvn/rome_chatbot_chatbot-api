namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Params;

    public interface IFunctionHandler
    {
        Task<Result> Handle(FunctionParam param);
    }

    public class FunctionHandler : IFunctionHandler
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders;

        public FunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders = null)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
        }

        public async Task<Result> Handle(FunctionParam param)
        {
            // Check Params

            if (!getDataHanlders.ContainsKey(param.Function.Id))
            {
                throw new InvalidOperationException($"Cannot find get data function for function: {param.Function.Id}-{param.Function.Name}");
            }

            var data = await getDataHanlders[param.Function.Id](param);

            if (data == null)
            {
                return new Result(0, "No data");
            }

            var message = messageBuilder.BuildCommandResultMessage(
                 param.User.Language,
                 param.Function.Id,
                 param.Messenger.Type,
                 data);

            return await messageSender.SendMessage(param.User.Id, param.Messenger, message, param.Function.Id);
        }
    }
}