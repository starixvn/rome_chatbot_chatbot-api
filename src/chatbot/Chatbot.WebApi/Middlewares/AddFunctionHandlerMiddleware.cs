namespace Chatbot.WebApi.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Configurations;
    using Chatbot.Params;
    using Microsoft.Extensions.DependencyInjection;

    public static class AddFunctionHandlerMiddleware
    {
        private static ReportService reportService;

        public static void AddFunctionHandler(
            this IServiceCollection services,
            ChatbotConfigs chatbotConfigs)
        {
            services.AddSingleton<IDictionary<int, Func<FunctionParam, Task<object>>>>(
                new Dictionary<int, Func<FunctionParam, Task<object>>>
            {
                { chatbotConfigs.OutstandingFunctionId, GetOutstanding },
                { chatbotConfigs.WinlossFunctionId, GetWinLoss },
                { chatbotConfigs.BalanceFunctionId, GetBalanceInfo },
                { chatbotConfigs.YesterdayWinLossFunctionId, GetYesterdayWinLoss }
            });
        }

        internal static ReportService ReportService
        {
            get
            {
                if (reportService == null)
                {
                    reportService = new ReportServiceImpl();
                }

                return reportService;
            }
            set
            {
                reportService = value;
            }
        }

        private static async Task<object> GetOutstanding(FunctionParam param)
           => await ReportService.GetOutstandingAsync(param.User.CustId);

        private static async Task<object> GetWinLoss(FunctionParam param)
            => await ReportService.GetWinLossAsync(param.User.CustId, DateTime.Now.Date);

        private static async Task<object> GetBalanceInfo(FunctionParam param)
            => await ReportService.GetBalanceInfoAsync(param.User.CustId);

        private static async Task<object> GetYesterdayWinLoss(FunctionParam param)
            => await ReportService.GetWinLossAsync(param.User.CustId, DateTime.Now.AddDays(-1).Date);
    }
}