namespace Chatbot.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Services;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Mvc;

    public class SettingController : Controller
    {
        private readonly SettingService settingService;

        public SettingController(SettingService settingService)
        {
            this.settingService = settingService;
        }

        [Route("api/setting/user"), HttpPost]
        public async Task<AddingUserResult> AddUser([FromBody]UserParam param)
            => await settingService.AddUser(
                param.CustId,
                param.SubAccountId,
                param.Language,
                new Messenger(param.MessengerType, param.MessengerUsername));

        [Route("api/setting/user"), HttpGet]
        public async Task<User> GetUser(int custId, int subAccountId, string language)
            => await settingService.GetUser(custId, subAccountId, language);

        [Route("api/setting/{userId}/function"), HttpPost]
        public async Task<Result> UpdateFunctions(int userId, [FromBody]IEnumerable<Function> functions)
        {
            await settingService.UpdateFunctions(userId, functions);

            return Result.CreateSuccessfulResult();
        }

        [Route("api/setting/{userId}/function/{language}"), HttpGet]
        public async Task<IEnumerable<Function>> GetFunctions(int userId, string language)
            => await settingService.GetFunctions(userId, language);

        [Route("api/setting/{userId}/language/{language}"), HttpPost]
        public async Task<Result> UpdateLanguage(int userId, string language)
        {
            await settingService.UpdateLanguage(userId, language);

            return Result.CreateSuccessfulResult();
        }

        [Route("api/setting/{userId}/generateVerificationCode"), HttpPost]
        public async Task<Result> GenerateVerificationCode(int userId, [FromBody]VerificationCodeParam param)
        {
            await settingService.GenerateVerificationCode(
                                userId,
                                new Messenger(param.MessengerType, param.MessengerUsername));

            return Result.CreateSuccessfulResult();
        }

    [Route("api/setting/{userId}/verifyVerificationCode"), HttpPost]
        public async Task<Result> VerifyVerificationCode(int userId, [FromBody]VerificationCodeParam param)
            => await settingService.VerifyVerificationCode(
                userId,
                param.VerificationCode,
                new Messenger(param.MessengerType, param.MessengerUsername));

        [Route("api/setting/{userId}/unsubscribe/{messengerType}/{messengerUsername}"), HttpPost]
        public async Task<Result> Unsubscribe(int userId, int messengerType, string messengerUsername)
        {
            await settingService.UpdateMessengerStatus(userId, messengerType, messengerUsername, false);

            return Result.CreateSuccessfulResult();
        }
}
}