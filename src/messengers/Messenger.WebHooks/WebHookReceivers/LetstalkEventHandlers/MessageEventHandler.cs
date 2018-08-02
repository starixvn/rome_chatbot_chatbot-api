namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        private readonly CommandService commandService;
        private readonly IMessengerClient messengerClient; 
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private const string eventTypeName = "message";

        public MessageEventHandler() : this(null, null, null)
        {
        }

        public MessageEventHandler(
            CommandService commandService,
            IMessengerClient messengerClient,
            ICryptographyHelper cryptographyHelper = null,
            IMessengerSettings messengerSettings = null)
        {
            this.messengerClient = messengerClient ?? DependencyResolver.GetInstance<IMessengerClient>();
            this.commandService = commandService ?? DependencyResolver.GetInstance<CommandService>();
            this.cryptographyHelper = cryptographyHelper ?? DependencyResolver.GetInstance<ICryptographyHelper>();
            this.messengerSettings = messengerSettings ?? DependencyResolver.GetInstance<IMessengerSettings>();
        }

        protected override string EventTypeName => eventTypeName;

        protected override async Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            var messageContent = cryptographyHelper.Decrypt(
                hookEvent.PayloadObject.HashContent,
                messengerSettings.ChatKey,
                hookEvent.PayloadObject.HashContentKey);

            var command = JsonConvert.DeserializeObject<MessageContent>(messageContent).Msg;

            await commandService.Run(command, MessengerType, hookEvent.Source.UserId);
        }
    }
}