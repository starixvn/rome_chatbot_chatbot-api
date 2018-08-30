namespace Chatbot.WebApi.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Configurations;
    using Chatbot.Modules.Funtions.Params;
    using Fanex.Data.Repository;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class AddFunctionHandlerMiddleware
    {
        private static ReportService reportService;
        private static MessageService messageService;
        private static AnnouncementService announcementService;
        private static Modules.Funtions.MessageService chatBotMessageService;

        public static void AddFunctionHandler(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = new ChatbotConfigs(serviceProvider.GetService<IConfiguration>());
            reportService = serviceProvider.GetService<ReportService>() ?? new ReportServiceImpl();
            messageService = serviceProvider.GetService<MessageService>() ?? new MessageServiceImpl();
            chatBotMessageService = serviceProvider.GetService<Modules.Funtions.MessageService>() ?? new Modules.Funtions.MessageServiceImpl(new DynamicRepository());
            announcementService = serviceProvider.GetService<AnnouncementService>() ?? new AnnouncementServiceImpl();

            services.AddSingleton<IDictionary<int, Func<FunctionParam, Task<object>>>>(
                new Dictionary<int, Func<FunctionParam, Task<object>>>
            {
                { chatbotConfigs.OutstandingFunctionId, GetOutstanding },
                { chatbotConfigs.WinlossFunctionId, GetWinLoss },
                { chatbotConfigs.BalanceFunctionId, GetBalanceInfo },
                { chatbotConfigs.YesterdayWinLossFunctionId, GetYesterdayWinLoss },
                { chatbotConfigs.PersonalMessageFunctionId, GetPersonalMessage },
                { chatbotConfigs.SpecialAnnouncementFunctionId, GetSpecialAnnouncement },
                { chatbotConfigs.NormalAnnouncementFunctionId, GetNormalAnnouncement },
                { chatbotConfigs.SystemUpdateFunctionId, GetSystemUpdate }
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
            => await GetMessage(param, MessageType.Personal);

        private static async Task<object> GetSpecialAnnouncement(FunctionParam param)
            => await GetMessage(param, MessageType.Special);

        private static async Task<object> GetNormalAnnouncement(FunctionParam param)
           => await GetMessage(param, MessageType.Normal);

        private static async Task<object> GetSystemUpdate(FunctionParam param)
        {
            var now = DateTime.Now;
            var lastQueryTime = await chatBotMessageService.GetLastQueryTime(param.User.Id, param.Function.Id, MessageType.System);
            var messages = announcementService.GetAnnouncements(param.User.CustId, param.User.Language, lastQueryTime, now);

            if (messages?.Any() == true)
            {
                await chatBotMessageService.UpdateLastQueryTime(param.User.Id, param.Function.Id, MessageType.System, now);
            }

            return messages;
        }

        private static async Task<IEnumerable<Message>> GetMessage(FunctionParam param, int messageType)
        {
            var now = DateTime.Now;
            var lastQueryTime = await chatBotMessageService.GetLastQueryTime(param.User.Id, param.Function.Id, messageType);
            var messages = await messageService.GetMessagesAsync(param.User.CustId, messageType, param.User.Language, lastQueryTime, now);

            if (messages?.Any() == true)
            {
                await chatBotMessageService.UpdateLastQueryTime(param.User.Id, param.Function.Id, messageType, now);
            }

            return messages;
        }
    }
}