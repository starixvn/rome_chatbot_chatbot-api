namespace Chatbot.Modules.Funtions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageLoggers;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Constants;
    using Fanex.Logging;

    public interface IFunctionHandlerFactory
    {
        IFunctionHandler CreateFunctionHandler(int functionId);
    }

    public class FunctionHandlerFactory : IFunctionHandlerFactory
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly CustomerService customerService;
        private readonly SettingService settingService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly UserLogActionService userLogActionService;
        private readonly ILogger logger;
        private readonly IMessageLogger messageLogger;

        public FunctionHandlerFactory(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders,
            IChatbotConfigs chatbotConfigs,
            CustomerService customerService,
            SettingService settingService,
            CopyCustomerService copyCustomerService,
            UserLogActionService userLogActionService,
            IMessageLogger messageLogger,
            ILogger logger)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
            this.customerService = customerService;
            this.settingService = settingService;
            this.copyCustomerService = copyCustomerService;
            this.userLogActionService = userLogActionService;
            this.logger = logger;
            this.messageLogger = messageLogger;
            this.chatbotConfigs = chatbotConfigs ?? throw new ArgumentNullException(nameof(chatbotConfigs));
        }

        public IFunctionHandler CreateFunctionHandler(int functionId)
        {
            if (chatbotConfigs.MessageAndAnnoucementFunctionIds.Contains(functionId))
            {
                return new MessageAnnouncementFunctionHandler(messageBuilder, messageSender, customerService, logger, settingService, getDataHanlders);
            }

            if (functionId == FunctionIds.YesterdayWinLoss)
            {
                return new YesterdayWinLossFunctionHandler(messageBuilder, messageSender, customerService, settingService, logger, getDataHanlders);
            }

            if (functionId == FunctionIds.Outstanding)
            {
                return new OutstandingFunctionHandler(chatbotConfigs, messageBuilder, messageSender, customerService, logger, getDataHanlders);
            }

            if (functionId == FunctionIds.CopyCustomer)
            {
                return new CopyCustomerFunctionHandler(
                    chatbotConfigs,
                    messageBuilder,
                    messageSender,
                    userLogActionService,
                    customerService,
                    copyCustomerService,
                    logger,
                    messageLogger,
                    getDataHanlders);
            }

            if (functionId == FunctionIds.BalanceInfo)
            {
                return new BalanceFunctionHandler(chatbotConfigs, messageBuilder, messageSender, customerService, logger, getDataHanlders);
            }

            return new DefaultFunctionHandler(messageBuilder, messageSender, customerService, logger, getDataHanlders);
        }
    }
}