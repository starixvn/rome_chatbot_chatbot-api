namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.Resources;
    using Chatbot.Shared.Configurations;

    public interface ActionNameBuilderFactory
    {
        IActionNameBuilder GetActionNameBuilder(int actionId);
    }

    public class ActionNameBuilderFactoryImpl : ActionNameBuilderFactory
    {
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly ResourceService resourceService;
        private readonly CopyCustomerService copyCustomerService;

        public ActionNameBuilderFactoryImpl(
            IChatbotConfigs chatbotConfigs,
            ResourceService resourceService,
            CopyCustomerService copyCustomerService)
        {
            this.chatbotConfigs = chatbotConfigs;
            this.resourceService = resourceService;
            this.copyCustomerService = copyCustomerService;
        }

        public IActionNameBuilder GetActionNameBuilder(int actionId)
        {
            if (chatbotConfigs.InputMaxCreditActionIds.Contains(actionId))
            {
                return new InputMaxCreditActionNameBuilder(resourceService, copyCustomerService);
            }

            return new DefaultActionNameBuilder(resourceService);
        }
    }
}