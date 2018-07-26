namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        private readonly IMessengerClient messengerClient;
        private readonly ICryptographyHelper cryptographyHelper;
        private readonly IMessengerSettings messengerSettings;
        private const string eventTypeName = "message";

        public MessageEventHandler() : this(null, null, null)
        {
        }

        public MessageEventHandler(
            IMessengerClient messengerClient = null,
            ICryptographyHelper cryptographyHelper = null,
            IMessengerSettings messengerSettings = null)
        {
            this.messengerClient = messengerClient ?? DependencyResolver.GetInstance<IMessengerClient>();
            this.cryptographyHelper = cryptographyHelper ?? DependencyResolver.GetInstance<ICryptographyHelper>();
            this.messengerSettings = messengerSettings ?? DependencyResolver.GetInstance<IMessengerSettings>();
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            var messageContent = cryptographyHelper.Decrypt(
                hookEvent.PayloadObject.HashContent,
                messengerSettings.ChatKey,
                hookEvent.PayloadObject.HashContentKey);

            var message = JsonConvert.DeserializeObject<MessageContent>(messageContent).Msg;

            return messengerClient.PushAsync(
                new List<string> { hookEvent.Source.UserId },
                $"Receive message from {hookEvent.Source.UserId}: {message} on {DateTime.Now}");

            return Task.FromResult(true);
        }
    }
}