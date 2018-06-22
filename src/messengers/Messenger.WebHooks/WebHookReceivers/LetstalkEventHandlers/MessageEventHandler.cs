namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Messenger.Client;
    using Messenger.Client.Models;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Helpers;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        private readonly IMessengerClient messengerClient;
        private const string eventTypeName = "message";

        public MessageEventHandler()
        {
            this.messengerClient = DependencyResolver.GetInstance<IMessengerClient>();
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            var requestContent = LogHelper.BuildRequestContent(HttpContext.Current.Request.Headers, hookEvent);
            Fanex.Logging.LogService.Info(requestContent);

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
                                HashContent = BuildMessage(hookEvent, requestContent).Replace("\r\n", "\n\n")
                            });

            Fanex.Logging.LogService.Info($"Request: Push, response: {JsonConvert.SerializeObject(result)}");

            return Task.FromResult(true);
        }

        private string BuildMessage(HookEvent hookEvent, string requestContent)
        {
            var message = $"Receive Message From: {hookEvent.Source.UserId}, Hook Event Type: {eventTypeName}, Payload: {hookEvent.EncryptedPayload}";

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine(message);
            contentBuilder.AppendLine("**Letstalk-ChatBot --> Messenger-WebHooks**");
            contentBuilder.Append(requestContent);

            return contentBuilder.ToString();
        }
    }
}