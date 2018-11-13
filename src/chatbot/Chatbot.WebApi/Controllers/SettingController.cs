namespace Chatbot.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.SettingLogs.Models;
    using Chatbot.Modules.Settings;
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
        public Task<AddingUserResult> AddUser([FromBody]UserParam param)
        {
            var logParam = param.LogParam;
            var messenger = new Messenger(param.MessengerType, param.MessengerUsername);
            var logData = new LinkAccountLog(
                logParam.ActorId,
                logParam.CustId,
                logParam.IP,
                messenger);

            return settingService.AddUser(
                  param.CustId,
                  param.SubAccountId,
                  param.CustName,
                  param.Language,
                  messenger,
                  param.CustLevel,
                  logData);
        }

        [Route("user"), HttpGet]
        public Task<User> GetUser(int custId, int subAccountId, string language)
            => settingService.GetUser(custId, subAccountId, language);

        [Route("{userId}/function"), HttpPost]
        public async Task<Result> UpdateFunctions(int userId, [FromBody]FunctionParam param)
        {
            var logParam = param.LogParam;
            var functions = await settingService.GetFunctions(userId);
            var logData = new UpdateFunctionsLog(logParam.ActorId, logParam.CustId, logParam.IP, param.Functions, functions);

            await settingService.UpdateFunctions(userId, param.Functions, logData);

            return Result.CreateSuccessfulResult();
        }

        [Route("{userId}/function/{language}"), HttpGet]
        public Task<IEnumerable<Function>> GetFunctions(int userId, string language)
            => settingService.GetFunctions(userId, language);

        [Route("{userId}/language/{language}"), HttpPost]
        public async Task<Result> UpdateLanguage(int userId, string language, [FromBody]LogParam logParam)
        {
            var user = await settingService.GetUser(userId);
            var logData = new UpdateLanguageLog(logParam.ActorId, logParam.CustId, logParam.IP, language, user.Language);

            await settingService.UpdateLanguage(userId, language, logData);

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
        {
            var logParam = param.LogParam;
            var messenger = new Messenger(param.MessengerType, param.MessengerUsername);
            var logData = new VerifyAccountLog(
                logParam.ActorId,
                logParam.CustId,
                logParam.IP,
                messenger);

            return await settingService.VerifyVerificationCode(
                userId,
                param.VerificationCode,
                messenger,
                logData);
        }

        [Route("{userId}/unsubscribe/{messengerType}/{messengerUsername}"), HttpPost]
        public async Task<Result> Unsubscribe(int userId, int messengerType, string messengerUsername, [FromBody]LogParam logParam)
        {
            var messenger = new Messenger(messengerType, messengerUsername);
            var logData = new UnlinkAccountLog(logParam.ActorId, logParam.CustId, logParam.IP, messenger);

            await settingService.Unsubscribe(userId, messenger, logData);

            return Result.CreateSuccessfulResult();
        }

        [Route("messenger/{messengerType}/{messengerUsername}"), HttpGet]
        public Task<MessengerDto> GetMessenger(int messengerType, string messengerUsername)
            => settingService.GetMessenger(messengerType, messengerUsername);

        [Route("users/function/{functionId}/{environment}"), HttpGet]
        public Task<IEnumerable<User>> GetUsersByFunction(int functionId, int environment)
            => settingService.GetUsersByFunction(functionId, environment);

        [Route("functions/{language}"), HttpGet]
        public async Task<IEnumerable<Function>> GetAllFunctions(string language)
            => await settingService.GetAllFunctions(language);

        [HttpPost("{userId}/selfdestructionPeriod/{minutes}")]
        public async Task<Result> UpdateSelfdestructionPeriod(int userId, int minutes, [FromBody]LogParam logParam)
        {
            var user = await settingService.GetUser(userId);
            var logData = new UpdateSelfdestructionPeriodLog(
                logParam.ActorId,
                logParam.CustId,
                logParam.IP,
                minutes,
                user.SelfdestructionPeriod);

            await settingService.UpdateSelfdestructionPeriod(userId, minutes, logData);

            return Result.CreateSuccessfulResult();
        }
    }
}