namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Shared.Configurations;
    using Fanex.Logging;

    public class BalanceFunctionHandler : DefaultFunctionHandler
    {
        private readonly IChatbotConfigs chatbotConfigs;

        public BalanceFunctionHandler(
            IChatbotConfigs chatbotConfigs,
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            ILogger logger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHandlers)
        {
            this.chatbotConfigs = chatbotConfigs;
        }

        protected internal override async Task<Result> RunHandler(FunctionParam param)
        {
            var parameter = GetParameter(param);

            if (!string.IsNullOrEmpty(parameter) && int.TryParse(parameter, out int topQuery))
            {
                if (topQuery < 0 || topQuery > chatbotConfigs.BalanceMaxTopQuery)
                {
                    return await BuildAndSendInvalidNumerOfDownlineRangeMessage(param, chatbotConfigs.BalanceMaxTopQuery)
                        .ConfigureAwait(false);
                }

                param.Others.Add(nameof(topQuery), topQuery);
            }

            return await base.RunHandler(param).ConfigureAwait(false);
        }

        private static string GetParameter(FunctionParam param)
        {
            var rawParameter = param.Others["parameters"] as string[];

            return rawParameter.Length > 0 ? rawParameter[0] : string.Empty;
        }
    }
}