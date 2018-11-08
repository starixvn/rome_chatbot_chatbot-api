namespace Chatbot.WebApi.Middlewares
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Shared.Configurations;
    using Microsoft.Extensions.DependencyInjection;

    public static class AddActionHandlerMiddleware
    {
        private static IChatbotConfigs chatbotConfigs;
        private static IMessageBuilder messageBuilder;
        private static CustomerService customerService;

        public static void AddActionHandler(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            chatbotConfigs = serviceProvider.GetService<IChatbotConfigs>();
            messageBuilder = serviceProvider.GetService<IMessageBuilder>();
            customerService = serviceProvider.GetService<CustomerService>();

            services.AddSingleton<IDictionary<int, IActionValidator>>(new Dictionary<int, IActionValidator>
            {
                [chatbotConfigs.OutstandingSpecificDirectDownlineActionId] = new DirectDownlineActionValidator(messageBuilder, customerService)
            });
        }
    }
}