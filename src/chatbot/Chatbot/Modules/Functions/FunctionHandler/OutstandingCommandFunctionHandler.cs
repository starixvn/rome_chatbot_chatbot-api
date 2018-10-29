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
            ILogger logger, IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHanlders)
        {
        }

        protected override async Task<Result> RunHandler(FunctionParam param)
        {
            var userName = param.Others["parameter"].ToString();

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var downlineInfo = await customerService.GetDownlineInfo(param.User.CustId, userName);

                if (downlineInfo == null || !IsDirectDownline(param.User.CustLevel, downlineInfo.RoleId))
                {
                    return await BuildAndSendInvalidCustNameMessage(param);
                }

                param.Others.Add(nameof(downlineInfo.CustId), downlineInfo.CustId);
            }

            return await base.RunHandler(param);
        }

        private bool IsDirectDownline(int custLevel, int downlineLevel)
            => (custLevel - downlineLevel) == 1;

        private Task<Result> BuildAndSendInvalidCustNameMessage(FunctionParam param)
        {
            var message = messageBuilder.BuildInvalidCustNameMessage(param.User.Language);

            return messageSender.SendMessage(param.User.Id, param.Messenger, message, param.Function.Id);
        }
    }
}