namespace Chatbot.Services.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Params;
    using Microsoft.Extensions.Configuration;

    public interface IFunctionHandlerFactory
    {
        IFunctionHandler CreateFunctionHandler(int functionId);
    }

    public class FunctionHandlerFactory : IFunctionHandlerFactory
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders;
        private readonly IConfiguration configuration;

        public FunctionHandlerFactory(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders,
            IConfiguration configuration)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHanlders = getDataHanlders;
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IFunctionHandler CreateFunctionHandler(int functionId)
        {
            var messageAndAnnouncementFunctionIds =
                    configuration.GetSection("Chatbot:MessageAndAnnoucementFunctionIds").Get<List<int>>()
                    ?? throw new InvalidOperationException($"Missing config for Chatbot:MessageAndAnnoucementFunctionIds");

            if (messageAndAnnouncementFunctionIds.Contains(functionId))
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