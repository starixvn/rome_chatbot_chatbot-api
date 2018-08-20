namespace Chatbot.WebApi.Middlewares
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using ONEbook.UM.AspNetCore.Services;
    using ONEbook.UM.Core.Models;

    public class UnderMaintenanceHandlingMiddleware
    {
        private const string JsonContentType = "application/json";
        private const string MaintenanceMessage = "Chatbot services are currently unavailable due to scheduled maintenance, from {0} to {1} (GMT+8).";
        private readonly RequestDelegate next;
        private readonly IMaintenanceService maintenanceService;

        public UnderMaintenanceHandlingMiddleware(
            RequestDelegate next,
            IMaintenanceService maintenanceService)
        {
            this.next = next;
            this.maintenanceService = maintenanceService;
        }

        public async Task Invoke(HttpContext context)
        {
            var umInfo = await maintenanceService.GetUnderMaintenanceTime();

            if (umInfo.IsUnderMaintenanceTime)
            {
                await HandleUnderMaintenance(context, umInfo);
            }
            else
            {
                await next(context);
            }
        }

        public Task HandleUnderMaintenance(HttpContext context, UnderMaintenanceTime umInfo)
        {
            var result = JsonConvert.SerializeObject(
                new
                {
                    error = string.Format(MaintenanceMessage, umInfo.From, umInfo.To)
                });

            context.Response.ContentType = JsonContentType;
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

            return context.Response.WriteAsync(result);
        }
    }
}