namespace Chatbot.Modules.Funtions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;

    public interface IFunctionHandler
    {
        Task<Result> Handle(FunctionParam param);
    }

    public class DefaultFunctionHandler : IFunctionHandler
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly CustomerService customerService;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders;

        protected bool SendMessageInCaseAccountClosed = true;

        public DefaultFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders = null)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
            this.customerService = customerService;
        }

        public virtual async Task<Result> Handle(FunctionParam param)
        {
            // Check Params

            if (!getDataHanlders.ContainsKey(param.Function.Id))
            {
                throw new InvalidOperationException($"Cannot find get data function for function: {param.Function.Id}-{param.Function.Name}");
            }

            if (await IsCustomerClosed(param.User))
            {
                return await BuildAndSendAccountClosedMessage(param);
            }

            return await RunHandler(param);
        }

        private async Task<Result> RunHandler(FunctionParam param)
        {
            var data = await getDataHanlders[param.Function.Id](param);

            if (data == null)
            {
                return Result.CreateSuccessfulResult("No data");
            }

            return await BuildAndSendMessage(param, data);
        }

        protected Task<bool> IsCustomerClosed(User user)
            => customerService.IsAccountClosed(user.CustId, user.SubAccountId);

        protected virtual Task<Result> BuildAndSendAccountClosedMessage(FunctionParam param)
        {
            if (SendMessageInCaseAccountClosed)
            {
                var message = messageBuilder.BuildAccountClosedMessage(param.User.Language);

                return messageSender.SendMessage(param.User.Id, param.Messenger, message, param.Function.Id);
            }

            return Task.FromResult(Result.CreateSuccessfulResult());
        }

        protected Task<Result> BuildAndSendMessage(FunctionParam param, object data)
        {
            var message = messageBuilder.BuildCommandResultMessage(
                 param.User.Language,
                 param.Function.Id,
                 param.Messenger.Type,
                 data);

            return messageSender.SendMessage(param.User.Id, param.Messenger, message, param.Function.Id);
        }
    }
}