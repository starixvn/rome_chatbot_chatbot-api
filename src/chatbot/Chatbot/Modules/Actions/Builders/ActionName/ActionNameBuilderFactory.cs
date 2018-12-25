namespace Chatbot.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using Chatbot.Modules.Resources;

    public interface ActionNameBuilderFactory
    {
        IActionNameBuilder GetActionNameBuilder(int actionId);
    }

    public class ActionNameBuilderFactoryImpl : ActionNameBuilderFactory
    {
        private readonly ResourceService resourceService;
        private readonly IDictionary<int, IActionNameBuilder> actionNameBuilders;

        public ActionNameBuilderFactoryImpl(
            ResourceService resourceService,
            IDictionary<int, IActionNameBuilder> actionNameBuilders)
        {
            this.resourceService = resourceService;
            this.actionNameBuilders = actionNameBuilders;
        }

        public IActionNameBuilder GetActionNameBuilder(int actionId)
            => actionNameBuilders.ContainsKey(actionId)
                ? actionNameBuilders[actionId]
                : new DefaultActionNameBuilder(resourceService);
    }
}