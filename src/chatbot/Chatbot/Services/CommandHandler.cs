namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Constants;
    using Chatbot.Models;

    public interface ICommandHandler
    {
        Task RunCommand(User user, Messenger messenger, IDictionary<string, object> parameters = null);
    }

    public abstract class CommandBaseHandler
    {
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;

        protected CommandBaseHandler(
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            IMessageLogger messageLogger)
        {
            this.messageBuilder = messageBuilder ?? new MessageBuilder();
            this.messageLogger = messageLogger ?? new MessageLogger();
            this.messageSender = messageSender ?? new MessageSender(new MessengerFactoryImpl());
        }

        protected async Task Handle(User user, Messenger messenger, object data)
        {
            var message = messageBuilder.BuildCommandResultMessage(data);

            await messageSender.SendMessage(messenger, message);
            await messageLogger.LogMessage(CommonId.BotUserId, user.Id, messenger, message);
        }
    }

    public class WinLossCommandHandler : CommandBaseHandler, ICommandHandler
    {
        private readonly ReportService reportService;

        public WinLossCommandHandler(
            ReportService reportService = null,
            IMessageSender messageSender = null,
            IMessageBuilder messageBuilder = null,
            IMessageLogger messageLogger = null)
            : base(
                    messageSender,
                    messageBuilder,
                    messageLogger
                  )
        {
            this.reportService = reportService ?? new ReportServiceImpl();
        }

        public async Task RunCommand(User user, Messenger messenger, IDictionary<string, object> parameters = null)
        {
            var winloss = await reportService.GetWinLossAsync(user.CustId, DateTime.Now);

            await Handle(user, messenger, winloss);
        }
    }

    public class BalanceInfoCommandHandler : CommandBaseHandler, ICommandHandler
    {
        private readonly ReportService reportService;

        public BalanceInfoCommandHandler(
            ReportService reportService = null,
            IMessageSender messageSender = null,
            IMessageBuilder messageBuilder = null,
            IMessageLogger messageLogger = null)
            : base(
                    messageSender,
                    messageBuilder,
                    messageLogger
                  )
        {
            this.reportService = reportService ?? new ReportServiceImpl();
        }

        public async Task RunCommand(User user, Messenger messenger, IDictionary<string, object> parameters = null)
        {
            var balanceInfo = await reportService.GetBalanceInfoAsync(user.CustId);

            await Handle(user, messenger, balanceInfo);
        }
    }

    public class OutstandingCommandHandler : CommandBaseHandler, ICommandHandler
    {
        private readonly ReportService reportService;

        public OutstandingCommandHandler(
            ReportService reportService = null,
            IMessageSender messageSender = null,
            IMessageBuilder messageBuilder = null,
            IMessageLogger messageLogger = null)
            : base(
                    messageSender,
                    messageBuilder,
                    messageLogger
                  )
        {
            this.reportService = reportService ?? new ReportServiceImpl();
        }

        public async Task RunCommand(User user, Messenger messenger, IDictionary<string, object> parameters = null)
        {
            var outstanding = await reportService.GetOutstandingAsync(user.CustId);

            await Handle(user, messenger, outstanding);
        }
    }
}