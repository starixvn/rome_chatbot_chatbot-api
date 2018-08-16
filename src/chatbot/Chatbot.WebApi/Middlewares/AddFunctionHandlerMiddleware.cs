namespace Chatbot.WebApi.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Configurations;
    using Chatbot.Params;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class AddFunctionHandlerMiddleware
    {
        private static ReportService reportService;
        private static MessageService messageService;
        private static Services.MessageService chatBotMessageService;

        public static void AddFunctionHandler(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = new ChatbotConfigs(serviceProvider.GetService<IConfiguration>());
            reportService = serviceProvider.GetService<ReportService>() ?? new ReportServiceImpl();
            messageService = serviceProvider.GetService<MessageService>() ?? new MessageServiceImpl();
            chatBotMessageService = serviceProvider.GetService<Services.MessageService>() ?? new Services.MessageServiceImpl(new DynamicRepository());

            services.AddSingleton<IDictionary<int, Func<FunctionParam, Task<object>>>>(
                new Dictionary<int, Func<FunctionParam, Task<object>>>
            {
                { chatbotConfigs.OutstandingFunctionId, GetOutstanding },
                { chatbotConfigs.WinlossFunctionId, GetWinLoss },
                { chatbotConfigs.BalanceFunctionId, GetBalanceInfo },
                { chatbotConfigs.YesterdayWinLossFunctionId, GetYesterdayWinLoss },
                { chatbotConfigs.PersonalMessageFunctionId, GetPersonalMessage }
            });
        }

        private static async Task<object> GetOutstanding(FunctionParam param)
           => await reportService.GetOutstandingAsync(param.User.CustId);

        private static async Task<object> GetWinLoss(FunctionParam param)
            => await reportService.GetWinLossAsync(param.User.CustId, DateTime.Now.Date);

        private static async Task<object> GetBalanceInfo(FunctionParam param)
            => await reportService.GetBalanceInfoAsync(param.User.CustId);

        private static async Task<object> GetYesterdayWinLoss(FunctionParam param)
            => await reportService.GetWinLossAsync(param.User.CustId, DateTime.Now.AddDays(-1).Date);

        private static async Task<object> GetPersonalMessage(FunctionParam param)
        {
            var now = DateTime.Now;
            var lastQueryTime = await chatBotMessageService.GetLastQueryTime(param.User.Id, param.Function.Id, MessageType.Personal);
            var message = await messageService.GetMessagesAsync(param.User.CustId, MessageType.Personal, param.User.Language, lastQueryTime, now);
            await chatBotMessageService.UpdateLastQueryTime(param.User.Id, param.Function.Id, MessageType.Personal, now);

            return message;
        }
    }
}