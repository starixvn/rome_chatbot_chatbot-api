namespace Chatbot.WebApi.Filters
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using ONEbook.UM.Services;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AutoReplyUMFilterAttribute : ActionFilterAttribute
    {
        private const string ParamName = "param";
        private readonly IMaintenanceService maintenanceService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;

        public AutoReplyUMFilterAttribute(
            IMaintenanceService maintenanceService,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder)
        {
            this.maintenanceService = maintenanceService;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey(ParamName))
            {
                var command = context.ActionArguments[ParamName] as CommandParam;
                if (command != null)
                {
                    var umInfo = await maintenanceService.GetUnderMaintenanceTime();

                    if (umInfo.IsUnderMaintenanceTime)
                    {
                        var umMessage = messageBuilder.BuildUnderMaintenanceMessage(umInfo.From, umInfo.To);
                        await messageSender.SendMessage(
                            new Models.Messenger(
                                command.MessengerType,
                                command.MessengerUsername),
                            umMessage,
                            logToDatabase: false);

                        await HandleUnderMaintenance(context.HttpContext, umMessage);

                        return;
                    }
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }

        public static Task HandleUnderMaintenance(HttpContext context, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

            return context.Response.WriteAsync(message);
        }
    }
}