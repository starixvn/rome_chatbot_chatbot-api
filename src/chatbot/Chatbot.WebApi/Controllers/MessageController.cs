namespace Chatbot.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageSender messageSender;
        private readonly SettingService settingService;
        private readonly FunctionService functionService;
        private readonly IMessageLogger messageLogger;

        public MessageController(
            IMessageSender messageSender,
            SettingService settingService,
            FunctionService functionService,
            IMessageLogger messageLogger)
        {
            this.messageSender = messageSender;
            this.settingService = settingService;
            this.functionService = functionService;
            this.messageLogger = messageLogger;
        }

        [Route("sendToSpecificMessenger"), HttpPost]
        public Task<Result> SendToSpecificMessenger([FromBody]MessageParam param)
            => messageSender.SendMessage(param.UserId, new Messenger(param.MessengerType, param.MessengerUsername), param.Message);

        [Route("send"), HttpPost]
        public async Task<Result> Send([FromBody]MessageParam param)
        {
            var messengers = await settingService.GetMessengers(param.UserId, true);

            return await messageSender.SendMessage(param.UserId, param.Message, messengers, param.FunctionId);
        }

        [Route("sendByFunction"), HttpPost]
        public Task<Result> SendByFunction([FromBody]FunctionParam param)
            => functionService.Handle(param.FunctionId, param.UserId);


        [Route("getMessageLogs"), HttpPost]
        public Task<IEnumerable<Message>> GetMessageLogs(MessageLogParam param)
            => messageLogger.GetMessageLogs(param.UserId, param.FromDate, param.ToDate, param.FunctionId, param.MessengerType);
    }
}