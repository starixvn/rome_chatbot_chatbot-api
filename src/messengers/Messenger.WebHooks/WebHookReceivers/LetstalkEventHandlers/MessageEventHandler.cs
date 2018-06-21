namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Bot.Client;
    using Messenger.Client;
    using Messenger.Client.Models;
    using Messenger.WebHooks.Helpers;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class MessageEventHandler : BaseLetstalkEventHandler
    {
        //private readonly IMessengerClient messengerClient;
        private const string eventTypeName = "message";

        private BotConnector botConnector;

        //public MessageEventHandler(IMessengerClient messengerClient)
        //{
        //    // Should use DI here
        //    this.messengerClient = messengerClient ?? new LetstalkClient();
        //}

        public MessageEventHandler()
        {
            botConnector = new BotConnector(new Fanex.Bot.Client.Models.BotSettings());
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            Fanex.Logging.LogService.Info(LogHelper.BuildRequestContent(HttpContext.Current.Request.Headers, hookEvent));

            var result = botConnector.SendAsync(hookEvent.EncryptedPayload, hookEvent.Source.UserId);

            Fanex.Logging.LogService.Info($"Request: Push, response: {JsonConvert.SerializeObject(result)}");

            return Task.FromResult(true);
        }
    }
}