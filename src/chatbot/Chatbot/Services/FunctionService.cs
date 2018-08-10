namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Params;

    public interface FunctionService
    {
        Task<Result> Handle(int functionId, int userId);
    }

    public class FunctionServiceImpl : FunctionService
    {
        private readonly SettingService settingService;
        private readonly IFunctionHandler functionHandler;

        public FunctionServiceImpl(SettingService settingService, IFunctionHandler functionHandler)
        {
            this.settingService = settingService;
            this.functionHandler = functionHandler;
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
                var param = new FunctionParam(user, function, messenger, null);

                results.Add(await functionHandler.Handle(param));
            }

            return results.All(result => result.IsOk)
               ? Result.CreateSuccessfulResult()
               : new Result(1);
        }
    }
}