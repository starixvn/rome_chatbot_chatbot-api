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
        public static void AddActionBuilders(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = serviceProvider.GetService<IChatbotConfigs>();
            var resourceService = serviceProvider.GetService<ResourceService>();
            var copyCustomerService = serviceProvider.GetService<CopyCustomerService>();
            var messageBuilder = serviceProvider.GetService<IMessageBuilder>();

            services.AddSingleton<IDictionary<int, IActionNameBuilder>>(
                new Dictionary<int, IActionNameBuilder>
                {
                    [chatbotConfigs.CopyMasterInputCreditActionId] = new InputMaxCreditActionNameBuilder(resourceService, copyCustomerService),
                    [chatbotConfigs.CopyAgentInputCreditActionId] = new InputMaxCreditActionNameBuilder(resourceService, copyCustomerService),
                    [chatbotConfigs.CopyAgentInputMemberCreditActionId] = new InputMemberMaxCreditActionNameBuilder(resourceService, chatbotConfigs),
                    [chatbotConfigs.CopyMemberInputCreditActionId] = new InputMaxCreditActionNameBuilder(resourceService, copyCustomerService)
                });

            services.AddSingleton<IDictionary<int, IActionTitleBuilder>>(
                new Dictionary<int, IActionTitleBuilder>
                {
                    [chatbotConfigs.CopyMasterInputCreditActionId] = new CopyMasterConfirmationTitleBuilder(chatbotConfigs, messageBuilder),
                    [chatbotConfigs.CopyAgentInputMemberCreditActionId] = new CopyAgentConfirmationTitleBuilder(chatbotConfigs, messageBuilder),
                    [chatbotConfigs.CopyMemberInputCreditActionId] = new CopyMemberConfirmationTitleBuilder(chatbotConfigs, messageBuilder)
                });

            services.AddSingleton<ActionNameBuilderFactory, ActionNameBuilderFactoryImpl>();
            services.AddSingleton<ActionTitleBuilderFactory, ActionTitleBuilderFactoryImpl>();
        }
    }
}