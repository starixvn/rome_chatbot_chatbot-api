namespace Chatbot.WebApi.Middlewares
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;
    using Microsoft.Extensions.DependencyInjection;

    public static class AddActionValidatorMiddleware
    {
        public static void AddActionValidators(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var chatbotConfigs = serviceProvider.GetService<IChatbotConfigs>();
            var messageBuilder = serviceProvider.GetService<IMessageBuilder>();
            var customerService = serviceProvider.GetService<CustomerService>();
            var copyCustomerService = serviceProvider.GetService<CopyCustomerService>();
            var settingService = serviceProvider.GetService<SettingService>();

            services.AddSingleton<IDictionary<int, IActionValidator>>(
                new Dictionary<int, IActionValidator>
                {
                    [chatbotConfigs.OutstandingSpecificDirectDownlineActionId] = new DirectDownlineActionValidator(messageBuilder, customerService),
                    [chatbotConfigs.CopyMasterInputUsernameActionId] = new DirectDownlineActionValidator(messageBuilder, customerService),
                    [chatbotConfigs.CopyMasterInputCreditActionId] = new MaxCreditValidator(messageBuilder, copyCustomerService),
                    [chatbotConfigs.CopyAgentInputUsernameActionId] = new DirectDownlineActionValidator(messageBuilder, customerService),
                    [chatbotConfigs.CopyAgentInputCreditActionId] = new MaxCreditValidator(messageBuilder, copyCustomerService),
                    [chatbotConfigs.CopyAgentInputMemberCreditActionId] = new MemberMaxCreditValidator(messageBuilder, chatbotConfigs),
                    [chatbotConfigs.CopyMemberInputUsernameActionId] = new DirectDownlineActionValidator(messageBuilder, customerService),
                    [chatbotConfigs.CopyMemberInputCreditActionId] = new MaxCreditValidator(messageBuilder, copyCustomerService),
                    [chatbotConfigs.CopyMasterActionId] = new CopyCustomerReachedLimitValidator(messageBuilder, settingService, chatbotConfigs),
                    [chatbotConfigs.CopyAgentActionId] = new CopyCustomerReachedLimitValidator(messageBuilder, settingService, chatbotConfigs),
                    [chatbotConfigs.CopyMemberActionId] = new CopyCustomerReachedLimitValidator(messageBuilder, settingService, chatbotConfigs)
                });
        }
    }
}