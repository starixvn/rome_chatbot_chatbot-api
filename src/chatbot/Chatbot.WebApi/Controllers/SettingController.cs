namespace Chatbot.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Services;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/setting")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly SettingService settingService;

        public SettingController(SettingService settingService)
        {
            this.settingService = settingService;
        }

        [Route("user"), HttpPost]
        public async Task<AddingUserResult> AddUser([FromBody]UserParam param)
            => await settingService.AddUser(
                param.CustId,
                param.SubAccountId,
                param.Language,
                new Messenger(param.MessengerType, param.MessengerUsername));

        [Route("user"), HttpGet]
        public async Task<User> GetUser(int custId, int subAccountId, string language)
            => await settingService.GetUser(custId, subAccountId, language);

        [Route("{userId}/function"), HttpPost]
        public async Task<Result> UpdateFunctions(int userId, [FromBody]IEnumerable<Function> functions)
        {
            await settingService.UpdateFunctions(userId, functions);

            return Result.CreateSuccessfulResult();
        }

        [Route("{userId}/function/{language}"), HttpGet]
        public async Task<IEnumerable<Function>> GetFunctions(int userId, string language)
            => await settingService.GetFunctions(userId, language);

        [Route("{userId}/language/{language}"), HttpPost]
        public async Task<Result> UpdateLanguage(int userId, string language)
        {
            await settingService.UpdateLanguage(userId, language);

            return Result.CreateSuccessfulResult();
        }

        [Route("{userId}/generateVerificationCode"), HttpPost]
        public async Task<Result> GenerateVerificationCode(int userId, [FromBody]VerificationCodeParam param)
        {
            await settingService.GenerateVerificationCode(
                                userId,
                                new Messenger(param.MessengerType, param.MessengerUsername));

            return Result.CreateSuccessfulResult();
        }

        [Route("{userId}/verifyVerificationCode"), HttpPost]
        public async Task<Result> VerifyVerificationCode(int userId, [FromBody]VerificationCodeParam param)
            => await settingService.VerifyVerificationCode(
                userId,
                param.VerificationCode,
                new Messenger(param.MessengerType, param.MessengerUsername));

        [Route("{userId}/unsubscribe/{messengerType}/{messengerUsername}"), HttpPost]
        public async Task<Result> Unsubscribe(int userId, int messengerType, string messengerUsername)
        {
            await settingService.UpdateMessengerStatus(userId, messengerType, messengerUsername, false);

            return Result.CreateSuccessfulResult();
        }

        [Route("messenger/{messengerType}/{messengerUsername}"), HttpGet]
        public async Task<MessengerDto> GetMessenger(int messengerType, string messengerUsername)
            => await settingService.GetMessenger(messengerType, messengerUsername);
    }
}