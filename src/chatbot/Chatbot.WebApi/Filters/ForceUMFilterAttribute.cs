namespace Chatbot.WebApi.Filters
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Shared.Configurations;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ForceUMFilterAttribute : ActionFilterAttribute
    {
        private const string ParamName = "param";
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IChatbotConfigs chatbotConfigs;

        public ForceUMFilterAttribute(
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.chatbotConfigs = chatbotConfigs;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey(ParamName)
                && chatbotConfigs.IsForceUM)
            {
                var command = context.ActionArguments[ParamName] as CommandParam;
                if (command != null)
                {
                    var umMessage = "I'm being upgraded now... please kindly talk to me later.";
                    await messageSender.SendMessage(
                        new Models.Messenger(
                            command.MessengerType,
                            command.MessengerUsername),
                        umMessage,
                        null,
                        logToDatabase: false);

                    await HandleUnderMaintenance(context.HttpContext, umMessage);

                    return;
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