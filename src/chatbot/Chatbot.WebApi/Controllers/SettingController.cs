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

        [Route("api/setting/{userId}/function"), HttpPost]
        public async Task<Result> UpdateFunctions(int userId, [FromBody]IEnumerable<Function> functions)
            => await settingService.UpdateFunctions(userId, functions);

        [Route("api/setting/{userId}/function"), HttpGet]
        public async Task<IEnumerable<Function>> GetFunctions(int userId)
            => await settingService.GetFunctions(userId);

        [Route("api/setting/{userId}/language/{language}"), HttpPost]
        public async Task<Result> UpdateLanguage(int userId, string language)
            => await settingService.UpdateLanguage(userId, language);

        [Route("api/setting/{userId}/generateVerificationCode"), HttpPost]
        public async Task<string> GenerateVerificationCode(int userId, [FromBody]VerificationCodeParam param)
            => await settingService.GenerateVerificationCode(
                userId, 
                new Messenger
                {
                    Account = param.MessengerAccount,
                    Type = param.MessengerType
                });

        [Route("api/setting/{userId}/verifyVerificationCode"), HttpPost]
        public async Task<Result> VerifyVerificationCode(int userId, [FromBody]VerificationCodeParam param)
            => await settingService.VerifyVerificationCode(
                userId,
                param.VerificationCode, 
                new Messenger
                {
                    Account = param.MessengerAccount,
                    Type = param.MessengerType
                });

        [Route("api/setting/addUser"), HttpPost]
        public async Task<AddingUserResult> AddUser([FromBody]UserParam param)
            => await settingService.AddUser(
                param.CustId,
                param.SubAccountId,
                param.Language,
                new Messenger
                {
                    Account = param.MessengerAccount,
                    Type = param.MessengerType
                });

        [Route("api/setting/user"), HttpGet]
        public async Task<User> GetUser(int custId, int subAccountId)
            => await settingService.GetUser(custId, subAccountId);

        [Route("api/setting/allfunctions"), HttpGet]
        public async Task<IEnumerable<Function>> GetAllFunctions()
            => await settingService.GetAllFunctions();

        [Route("api/setting/{userId}/unsubscribe/{messengerType}"), HttpPost]
        public async Task<Result> Unsubscribe(int userId, int messengerType)
            => await settingService.Unsubscribe(userId, messengerType);
    }
}