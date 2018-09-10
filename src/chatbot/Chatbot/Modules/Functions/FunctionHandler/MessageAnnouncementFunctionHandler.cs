namespace Chatbot.Modules.Funtions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Helpers;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;

    public class MessageAnnouncementFunctionHandler : DefaultFunctionHandler
    {
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;
        private readonly SettingService settingService;

        public MessageAnnouncementFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            ILogger logger,
            SettingService settingService,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null) :
                base(messageBuilder, messageSender, customerService, logger, getDataHandlers)
        {
            this.getDataHandlers = getDataHandlers;
            SendMessageInCaseAccountClosed = false;
            this.settingService = settingService;
        }

        public override async Task<IEnumerable<FunctionMessage>> BuildFunctionMessages(FunctionParam param)
        {
            var functionMessages = new List<FunctionMessage>();

            var data = await getDataHandlers[param.Function.Id](param);
            var announcements = DataHelper.TryParse<IList<Alpha.Services.Client.Models.Message>>(data);

            if (announcements == null || announcements.Count == 0)
            {
                return new List<FunctionMessage>();
            }

            foreach (var announcement in announcements)
            {
                if (!string.IsNullOrEmpty(announcement.Content))
                {
                    var user = param.Users.FirstOrDefault(u => u.CustId == announcement.CustId);

                    if (await IsCustomerClosed(user))
                    {
                        continue;
                    }

                    var messengers = await settingService.GetMessengers(user.Id, true);

                    foreach (var messenger in messengers)
                    {
                        var message = BuildMessage(user.Language, param.Function.Id, messenger.Type, announcement);

                        functionMessages.Add(new FunctionMessage(user.Id, message));
                    }
                }
            }

            return functionMessages;
        }
    }
}