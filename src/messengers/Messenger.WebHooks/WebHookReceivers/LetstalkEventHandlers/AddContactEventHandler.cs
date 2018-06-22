namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Messenger.Client;
    using Messenger.Client.Models;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Helpers;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class AddContactEventHandler : BaseLetstalkEventHandler
    {
        private const string eventTypeName = "add_contact";
        private readonly IMessengerClient messengerClient;

        public AddContactEventHandler()
        {
            messengerClient = DependencyResolver.GetInstance<IMessengerClient>();
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            Fanex.Logging.LogService.Info(LogHelper.BuildRequestContent(HttpContext.Current.Request.Headers, hookEvent));

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
                                HashContent = $"Receive Message From: {hookEvent.Source.UserId}, Hook Event Type: {eventTypeName}"
                            });

            Fanex.Logging.LogService.Info($"Request: Push, response: {JsonConvert.SerializeObject(result)}");

            return Task.FromResult(true);
        }
    }
}