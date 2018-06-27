namespace Messenger.WebHooks.WebHookReceivers.LetstalkEventHandlers
{
    using System.Threading.Tasks;
    using System.Web;
    using Messenger.Client;
    using Messenger.WebHooks.App_Start;
    using Messenger.WebHooks.Helpers;
    using Messenger.WebHooks.Models;
    using Microsoft.AspNet.WebHooks;

    public class HealthCheckEventHandler : BaseLetstalkEventHandler
    {
        private const string eventTypeName = "health_check";
        private readonly IMessengerClient messengerClient;

        public HealthCheckEventHandler()
        {
            messengerClient = DependencyResolver.GetInstance<IMessengerClient>();
        }

        protected override string EventTypeName => eventTypeName;

        protected override Task EnqueueAsync(WebHookQueueContext context, HookEvent hookEvent)
        {
            // Log first, implement real business later
            Fanex.Logging.LogService.Info(LogHelper.BuildRequestContent(HttpContext.Current.Request.Headers, hookEvent));

            return Task.FromResult(true);
        }
    }
}