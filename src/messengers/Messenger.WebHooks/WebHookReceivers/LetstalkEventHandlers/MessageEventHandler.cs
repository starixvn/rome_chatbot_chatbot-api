namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client;
    using Messenger.Client.Models;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        private readonly IMessengerClient messengerClient;
        private const string eventTypeName = "message";

        public MessageEventHandler(IMessengerClient messengerClient)
        {
            // Should use DI here
            this.messengerClient = messengerClient ?? new LetstalkClient();
        }

        public MessageEventHandler() : this(null)
        {
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(hookEvent, Formatting.Indented));

            var result = messengerClient.Push(
                            "botName",
                            new List<string>
                            {
                                hookEvent.Source.UserId
                            },
                            new Payload
                            {
                                Id = 1,
                                From = 0,
                                UserId = hookEvent.Source.UserId,
                                HashContent = $"Receive Message From: {hookEvent.Source.UserId}, Hook Event Type: {eventTypeName}, Payload: {hookEvent.EncryptedPayload}"
                            });

            Fanex.Logging.LogService.Info($"Request: Push, response: {JsonConvert.SerializeObject(result)}");

            return Task.FromResult(true);
        }
    }
}