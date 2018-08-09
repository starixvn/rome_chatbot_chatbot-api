namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Configurations;
    using Chatbot.Constants;
    using Chatbot.Params;

    public interface ICommandHandler
    {
        Task RunCommand(CommandParam param);
    }

    public class CommandHandler : ICommandHandler
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IMessageSender messageSender;
        private readonly IDictionary<int, Func<CommandParam, Task<object>>> getDataHanlders;

        public CommandHandler(
            IMessageBuilder messageBuilder,
            IChatbotConfigs chatbotConfigs,
            IMessageSender messageSender,
            IDictionary<int, Func<CommandParam, Task<object>>> getDataHanlders = null)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.chatbotConfigs = chatbotConfigs;
            this.getDataHanlders = getDataHanlders ?? new Dictionary<int, Func<CommandParam, Task<object>>>
            {
                { chatbotConfigs.OutstandingFunctionId, GetOutstanding },
                { chatbotConfigs.WinlossFunctionId, GetWinLoss },
                { chatbotConfigs.BalanceFunctionId, GetBalanceInfo }
            };
        }

        private async Task<object> GetOutstanding(CommandParam param)
            => await (new ReportServiceImpl()).GetOutstandingAsync(param.User.CustId);

        private async Task<object> GetWinLoss(CommandParam param)
            => await (new ReportServiceImpl()).GetWinLossAsync(param.User.CustId, DateTime.Now.AddDays(-1).Date);

        private async Task<object> GetBalanceInfo(CommandParam param)
            => await (new ReportServiceImpl()).GetBalanceInfoAsync(param.User.CustId);

        public async Task RunCommand(CommandParam param)
        {
            // Check Params

            if (!getDataHanlders.ContainsKey(param.Function.Id))
            {
                throw new InvalidOperationException($"Cannot find get data function for function: {param.Function.Id}-{param.Function.Name}");
            }

            var data = await getDataHanlders[param.Function.Id](param);

            await Handle(param, data);
        }

        private async Task Handle(CommandParam param, object data)
        {
            var message = messageBuilder.BuildCommandResultMessage(
                param.User.Language,
                param.Function.Id,
                param.Messenger.Type,
                data);

            await messageSender.SendMessage(param.User.Id, param.Messenger, message, param.Function.Id);
        }
    }
}