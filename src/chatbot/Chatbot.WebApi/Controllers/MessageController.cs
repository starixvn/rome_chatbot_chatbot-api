namespace Chatbot.WebApi.Controllers
{
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Services;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageSender messageSender;
        private readonly SettingService settingService;

        public MessageController(
            IMessageSender messageSender,
            SettingService settingService)
        {
            this.messageSender = messageSender;
            this.settingService = settingService;
        }

        [Route("sendToSpecificMessenger"), HttpPost]
        public async Task<Result> SendToSpecificMessenger([FromBody]MessageParam param)
            => await messageSender.SendMessage(param.UserId, new Messenger(param.MessengerType, param.MessengerUsername), param.Message);

        [Route("send"), HttpPost]
        public async Task<Result> Send([FromBody]MessageParam param)
        {
            var messengers = await settingService.GetMessengers(param.UserId, true);

            return await messageSender.SendMessage(param.UserId, param.Message, messengers, param.FunctionId);
        }
    }
}