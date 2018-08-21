namespace Chatbot.Modules.Funtions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;

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

        public FunctionHandlerFactory(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders,
            IChatbotConfigs chatbotConfigs,
            CustomerService customerService)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
            this.customerService = customerService;
            this.chatbotConfigs = chatbotConfigs ?? throw new ArgumentNullException(nameof(chatbotConfigs));
        }

        public IFunctionHandler CreateFunctionHandler(int functionId)
        {
            if (chatbotConfigs.MessageAndAnnoucementFunctionIds.Contains(functionId))
            {
                return new MessageAnnouncementFunctionHandler(messageBuilder, messageSender, customerService, getDataHanlders);
            }

            return new DefaultFunctionHandler(messageBuilder, messageSender, customerService, getDataHanlders);
        }
    }
}