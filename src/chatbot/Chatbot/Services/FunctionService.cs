namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Params;
    using Microsoft.Extensions.Logging;

    public interface FunctionService
    {
        Task<Result> Handle(int functionId, int userId);
    }

    public class FunctionServiceImpl : FunctionService
    {
        private readonly SettingService settingService;
        private readonly IFunctionHandler functionHandler;
        private readonly ILogger<FunctionServiceImpl> logger;

        public FunctionServiceImpl(
            SettingService settingService,
            IFunctionHandler functionHandler,
            ILogger<FunctionServiceImpl> logger)
        {
            this.settingService = settingService;
            this.functionHandler = functionHandler;
            this.logger = logger;
        }

        public async Task<Result> Handle(int functionId, int userId)
        {
            var results = new List<Result>();

            var messengers = await settingService.GetMessengers(userId, true);

            var user = await settingService.GetUser(userId)
                 ?? throw new InvalidOperationException($"User cannot be found");

            var function = (await settingService.GetFunctions(userId)).FirstOrDefault(f => f.Id == functionId)
                 ?? throw new InvalidOperationException($"Function cannot be found for user {userId}");

            foreach (var messenger in messengers)
            {
                try
                {
                    var param = new FunctionParam(user, function, messenger, null);

                    results.Add(await functionHandler.Handle(param));
                }
                catch (Exception ex)
                {
                    LogError(user, messenger, ex);
                }
            }

            return results.All(result => result.IsOk)
               ? Result.CreateSuccessfulResult()
               : new Result(1);
        }

        private void LogError(User user, Messenger messenger, Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Function Service Exception",
                                    $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                                    $"Messenger: {messenger.Username}, Type: {messenger.Type}",
                                    ex.ToString());

            logger.LogError(logMessage);
        }
    }
}