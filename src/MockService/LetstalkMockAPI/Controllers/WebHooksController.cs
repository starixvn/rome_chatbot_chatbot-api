namespace LetstalkMockAPI.Controllers
{
    using System.Web.Http;
    using LetstalkMockAPI.Configurations;
    using LetstalkMockAPI.Models;
    using LetstalkMockAPI.Models.WebHookEvents;
    using Microsoft.AspNet.SignalR.Client;

    public class WebHooksController : ApiController
    {
        private IHubProxy chatRHubProxy;

        public WebHooksController()
        {
            var hubConnection = new HubConnection(AppSettings.MessengerAppUrl);
            chatRHubProxy = hubConnection.CreateHubProxy("ChatHub");
            hubConnection.Start().Wait();
        }

        [HttpPost]
        [Route("api/webhooks/incoming")]
        public void Incoming(MessageEvent body)
        {
            chatRHubProxy.Invoke<ChatMessage>(
                "Receive",
                new ChatMessage
                {
                    Username = body.Payload.UserId,
                    Content = body.Payload.HashContent
                });
        }
    }
}