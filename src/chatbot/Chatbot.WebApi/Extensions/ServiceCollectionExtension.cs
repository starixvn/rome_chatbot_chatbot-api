namespace Chatbot.WebApi.Extensions
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Actions.Builders.ActionTitle;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Resources;
    using Chatbot.Shared.Configurations;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtension
    {
        public static void AddActionNameBuilders(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = serviceProvider.GetService<IChatbotConfigs>();
            var resourceService = serviceProvider.GetService<ResourceService>();
            var copyCustomerService = serviceProvider.GetService<CopyCustomerService>();

            services.AddSingleton<IDictionary<int, IActionNameBuilder>>(new Dictionary<int, IActionNameBuilder>
            {
                [chatbotConfigs.CopyMasterInputCreditActionId] = new InputMaxCreditActionNameBuilder(resourceService, copyCustomerService)
            });
        }

        public static void AddActionTilteBuilders(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = serviceProvider.GetService<IChatbotConfigs>();
            var messageBuilder = serviceProvider.GetService<IMessageBuilder>();

            services.AddSingleton<IDictionary<int, IActionTitleBuilder>>(new Dictionary<int, IActionTitleBuilder>
            {
                [chatbotConfigs.CopyMasterInputCreditActionId] = new CopyAgentConfirmationTitleBuilder(chatbotConfigs, messageBuilder)
            });
        }
    }
}