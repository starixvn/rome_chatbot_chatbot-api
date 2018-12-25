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
    using Chatbot.Shared.Configurations;
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
            IMessageLogger messageLogger,
            ILogger logger)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
            this.customerService = customerService;
            this.settingService = settingService;
            this.copyCustomerService = copyCustomerService;
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

            if (functionId == chatbotConfigs.YesterdayWinLossFunctionId)
            {
                return new YesterdayWinLossFunctionHandler(messageBuilder, messageSender, customerService, settingService, logger, getDataHanlders);
            }

            if (functionId == chatbotConfigs.OutstandingFunctionId)
            {
                return new OutstandingFunctionHandler(chatbotConfigs, messageBuilder, messageSender, customerService, logger, getDataHanlders);
            }

            if (functionId == chatbotConfigs.CopyCustomerFunctionId)
            {
                return new CopyCustomerFunctionHandler(chatbotConfigs, messageBuilder, messageSender, settingService, customerService, copyCustomerService, logger, messageLogger, getDataHanlders);
            }

            if (functionId == chatbotConfigs.BalanceFunctionId)
            {
                return new BalanceFunctionHandler(chatbotConfigs, messageBuilder, messageSender, customerService, logger, getDataHanlders);
            }

            return new DefaultFunctionHandler(messageBuilder, messageSender, customerService, logger, getDataHanlders);
        }
    }
}