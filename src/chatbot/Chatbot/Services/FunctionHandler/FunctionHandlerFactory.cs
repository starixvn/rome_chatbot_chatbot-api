namespace Chatbot.Services.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Params;

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

        public FunctionHandlerFactory(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
            this.chatbotConfigs = chatbotConfigs ?? throw new ArgumentNullException(nameof(chatbotConfigs));
        }

        public IFunctionHandler CreateFunctionHandler(int functionId)
        {
            if (chatbotConfigs.MessageAndAnnoucementFunctionIds.Contains(functionId))
            {
                return new MessageAnnouncementFunctionHandler(messageBuilder, messageSender, getDataHanlders);
            }
            else
            {
                return new DefaultFunctionHandler(messageBuilder, messageSender, getDataHanlders);
            }
        }
    }
}