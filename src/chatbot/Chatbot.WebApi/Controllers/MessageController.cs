namespace Chatbot.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
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
            => messageSender.SendMessage(
                param.UserId,
                new Messenger(param.MessengerType, param.MessengerUsername),
                param.Message,
                param.SelfdestructionPeriod);

        [Route("send"), HttpPost]
        public async Task<Result> Send([FromBody]MessageParam param)
        {
            var messengers = await settingService.GetMessengers(param.UserId, true);

            return await messageSender.SendMessage(param.UserId, param.Message, messengers, param.SelfdestructionPeriod, param.FunctionId);
        }

        [Route("getFunctionMessages"), HttpGet]
        public Task<IEnumerable<FunctionMessage>> GetFunctionMessages([FromQuery]int functionId)
            => functionService.GetFunctionMessages(functionId);

        [Route("getMessageLogs"), HttpPost]
        public Task<IEnumerable<Message>> GetMessageLogs(MessageLogParam param)
            => messageLogger.GetMessageLogs(
                param.UserId,
                DateTimeOffset.FromUnixTimeMilliseconds(param.FromDate).DateTime.ToLocalTime(),
                DateTimeOffset.FromUnixTimeMilliseconds(param.ToDate).DateTime.ToLocalTime(),
                param.FunctionId,
                param.MessengerType);
    }
}