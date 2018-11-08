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
    using Fanex.Logging;

    public class OutstandingCommandFunctionHandler : DefaultFunctionHandler
    {
        public OutstandingCommandFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            ILogger logger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHandlers)
        {
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
                        param.Others.Add(nameof(topQuery), topQuery);
                    }
                    else
                    {
                        var downlineInfo = await customerService.GetDownlineInfo(param.User.CustId, parameter)
                            .ConfigureAwait(false);

                        if (downlineInfo == null ||
                            downlineInfo.CustId == 0 ||
                            !IsDirectDownline(param.User.CustLevel, downlineInfo.RoleId))
                        {
                            return await BuildAndSendInvalidCustNameMessage(param).ConfigureAwait(false);
                        }

                        param.Others.Add(nameof(downlineInfo.CustId), downlineInfo.CustId);
                    }
                }
            }

            return await base.RunHandler(param).ConfigureAwait(false);
        }

        private Task<Result> BuildAndSendInvalidCustNameMessage(FunctionParam param)
        {
            var message = messageBuilder.BuildInvalidCustNameMessage(param.User.Language);

            return messageSender.SendMessage(param.User.Id, param.Messenger, message, param.Function.Id);
        }

        private static bool IsDirectDownline(int custLevel, int downlineLevel)
            => (custLevel - downlineLevel) == 1;
    }
}