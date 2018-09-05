namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;

    public interface FunctionService
    {
        Task<Result> Handle(int functionId, int userId);
    }

    public class FunctionServiceImpl : FunctionService
    {
        private readonly SettingService settingService;
        private readonly IFunctionHandlerFactory functionHandlerFactory;
        private readonly ILogger logger;

        public FunctionServiceImpl(
            SettingService settingService,
            IFunctionHandlerFactory functionHandlerFactory,
            ILogger logger)
        {
            this.settingService = settingService;
            this.functionHandlerFactory = functionHandlerFactory;
            this.logger = logger;
        }

        public async Task<Result> Handle(int functionId, int userId)
        {
            var results = new List<Result>();

            var user = await settingService.GetUser(userId)
                ?? throw new InvalidOperationException($"User cannot be found");

            var messengers = await settingService.GetMessengers(userId, true);

            var function = (await settingService.GetFunctions(userId)).FirstOrDefault(f => f.Id == functionId)
                ?? throw new InvalidOperationException($"Function cannot be found for user {userId}");

            foreach (var messenger in messengers)
            {
                try
                {
                    var param = new FunctionParam(user, function, messenger, null);
                    var result = await functionHandlerFactory
                                        .CreateFunctionHandler(functionId)
                                        .Handle(param);

                    results.Add(result);
                }
                catch (Exception ex)
                {
                    LogError(user, messenger, ex);

                    return Result.CreateErrorResult();
                }
            }

            return results.All(result => result.IsOk)
               ? Result.CreateSuccessfulResult()
               : Result.CreateErrorResult();
        }

        private void LogError(User user, Messenger messenger, Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Function Service Exception",
                                    $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                                    $"Messenger: {messenger.Username}, Type: {messenger.Type}",
                                    ex.ToString());

            logger.Error(logMessage);
        }
    }
}