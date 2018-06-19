namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Threading.Tasks;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;
    using Newtonsoft.Json;

    public class AskKeyEventHandler : BaseLetstalkEventHandler
    {
        private const string eventTypeName = "ask_key";

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            Fanex.Logging.LogService.Info(JsonConvert.SerializeObject(hookEvent, Formatting.Indented));

            return Task.FromResult(true);
        }
    }
}