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
        public static void AddActionHandler(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = serviceProvider.GetService<IChatbotConfigs>();
            var messageBuilder = serviceProvider.GetService<IMessageBuilder>();
            var customerService = serviceProvider.GetService<CustomerService>();
            var copyCustomerService = serviceProvider.GetService<CopyCustomerService>();

            services.AddSingleton<IDictionary<int, IActionValidator>>(
                new Dictionary<int, IActionValidator>
                {
                    [chatbotConfigs.OutstandingSpecificDirectDownlineActionId] = new DirectDownlineActionValidator(messageBuilder, customerService),
                    [chatbotConfigs.CopyMasterInputUsernameActionId] = new DirectDownlineActionValidator(messageBuilder, customerService),
                    [chatbotConfigs.CopyMasterInputCreditActionId] = new MaxCreditValidator(messageBuilder, copyCustomerService)
                });
        }
    }
}