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
    using Chatbot.Modules.Settings;
    using Fanex.Logging;

    public class YesterdayWinLossFunctionHandler : DefaultFunctionHandler
    {
        private readonly SettingService settingService;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;

        public YesterdayWinLossFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender, CustomerService customerService,
            SettingService settingService,
            ILogger logger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHandlers)
        {
            this.settingService = settingService;
            this.getDataHandlers = getDataHandlers;
        }

        /// <summary>
        /// Don't send closed account message for schedule functions
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected override Task<Result> BuildAndSendAccountClosedMessage(FunctionParam param)
            => Task.FromResult(Result.CreateSuccessfulResult("Account is closed"));

        public override async Task<IEnumerable<FunctionMessage>> GetFunctionMessages(FunctionParam param)
        {
            var functionMessages = new List<FunctionMessage>();

            foreach (var user in param.Users)
            {
                try
                {
                    if (await IsCustomerClosed(user).ConfigureAwait(false))
                    {
                        continue;
                    }

                    var messengers = await settingService.GetMessengers(user.Id, true).ConfigureAwait(false);
                    await BuildFunctionMessages(param, functionMessages, user, messengers).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogError(user, ex);
                }
            }

            return functionMessages;
        }

        private async Task BuildFunctionMessages(
            FunctionParam param,
            List<FunctionMessage> functionMessages,
            User user,
            IEnumerable<Messenger> messengers)
        {
            foreach (var messenger in messengers)
            {
                var data = await getDataHandlers[param.Function.Id](new FunctionParam(user, param.Function, messenger, null))
                    .ConfigureAwait(false);

                if (data != null)
                {
                    var message = BuildMessage(user.Language, param.Function.Id, messenger.Type, data);

                    functionMessages.Add(new FunctionMessage(user.Id, message, user.SelfdestructionPeriod));
                }
            }
        }
    }
}