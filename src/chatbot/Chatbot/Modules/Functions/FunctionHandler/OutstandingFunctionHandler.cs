namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Shared.Configurations;
    using Fanex.Logging;
    using Result = Models.Result;

    public class OutstandingFunctionHandler : DefaultFunctionHandler
    {
        private readonly IChatbotConfigs chatbotConfigs;

        public OutstandingFunctionHandler(
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

        protected override async Task<Result> RunHandler(FunctionParam param)
        {
            var rawParameter = param.Others["parameters"] as string[];
            if (rawParameter.Length > 0)
            {
                var parameter = rawParameter[0];

                if (!string.IsNullOrWhiteSpace(parameter))
                {
                    if (int.TryParse(parameter, out int topQuery))
                    {
                        if (topQuery < 0 || topQuery > chatbotConfigs.OutstandingMaxTopQuery)
                        {
                            return await BuildAndSendInvalidNumerOfDownlineRangeMessage(param, chatbotConfigs.OutstandingMaxTopQuery)
                                .ConfigureAwait(false);
                        }

                        param.Others.Add(nameof(topQuery), topQuery);
                    }
                    else
                    {
                        var downlineInfo = await customerService.GetDownlineInfo(param.User.CustId, parameter)
                            .ConfigureAwait(false);

                        if (!IsValidDirectDownline(param.User.CustLevel, downlineInfo))
                        {
                            return await BuildAndSendInvalidCustNameMessage(param).ConfigureAwait(false);
                        }

                        param.Others.Add(nameof(downlineInfo.CustId), downlineInfo.CustId);
                        param.Others.Add("CustName", parameter);
                    }
                }
            }

            return await base.RunHandler(param).ConfigureAwait(false);
        }

        private Task<Result> BuildAndSendInvalidCustNameMessage(FunctionParam param)
        {
            var message = messageBuilder.BuildInvalidCustNameMessage(param.User.Language);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }
    }
}