namespace Chatbot.WebApi.Controllers
{
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/letstalk")]
    [ApiController]
    public class LetstalkClientController : ControllerBase
    {
        private readonly MessengerFactory messengerFactory;

        public LetstalkClientController(MessengerFactory messengerFactory)
        {
            this.messengerFactory = messengerFactory;
        }

        [Route("setwebhook/{webhookurl}"), HttpPost]
        public async Task<Result> SetWebhook(string webhookUrl)
        {
            var messengerClient = messengerFactory.CreateMessengerClient(MessengerType.Letstalk);

            var result = await messengerClient.SetWebhookAsync(webhookUrl);

            return new Result(result.Status, result.Message);
        }
    }
}