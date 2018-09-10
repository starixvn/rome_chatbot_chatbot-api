namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;

    public class YesterdayWinLossFunctionHandler : DefaultFunctionHandler
    {
        private readonly SettingService settingService;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders;

        public YesterdayWinLossFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender, CustomerService customerService,
            SettingService settingService,
            ILogger logger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHanlders = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHanlders)
        {
            this.settingService = settingService;
            this.getDataHanlders = getDataHanlders;
        }

        /// <summary>
        /// Don't send closed account message for schedule functions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Task<Result> BuildAndSendAccountClosedMessage(FunctionParam param)
            => Task.FromResult(Result.CreateSuccessfulResult("Account is closed"));

        public override async Task<IEnumerable<FunctionMessage>> BuildFunctionMessages(FunctionParam param)
        {
            var functionMessages = new List<FunctionMessage>();

            foreach (var user in param.Users)
            {
                if (await IsCustomerClosed(user))
                {
                    continue;
                }

                var messengers = await settingService.GetMessengers(user.Id, true);

                foreach (var messenger in messengers)
                {
                    var data = await getDataHanlders[param.Function.Id](new FunctionParam(user, param.Function, messenger, null));

                    if (data != null)
                    {
                        var message = BuildMessage(user.Language, param.Function.Id, messenger.Type, data);

                        functionMessages.Add(new FunctionMessage(user.Id, message));
                    }
                }
            }

            return functionMessages;
        }
    }
}