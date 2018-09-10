namespace Chatbot.Modules.Funtions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;

    public interface FunctionService
    {
        Task<IEnumerable<FunctionMessage>> GetFunctionMessages(int functionId);
    }

    public class FunctionServiceImpl : FunctionService
    {
        private readonly SettingService settingService;
        private readonly IFunctionHandlerFactory functionHandlerFactory;

        public FunctionServiceImpl(
            SettingService settingService,
            IFunctionHandlerFactory functionHandlerFactory)
        {
            this.settingService = settingService;
            this.functionHandlerFactory = functionHandlerFactory;
        }

        public async Task<IEnumerable<FunctionMessage>> GetFunctionMessages(int functionId)
        {
            var users = await settingService.GetUsersByFunction(functionId, 0).ConfigureAwait(false);

            var param = new FunctionParam(null, new Function { Id = functionId }, null, null, users);

            return await functionHandlerFactory
                            .CreateFunctionHandler(functionId)
                            .BuildFunctionMessages(param)
                            .ConfigureAwait(false);
        }
    }
}