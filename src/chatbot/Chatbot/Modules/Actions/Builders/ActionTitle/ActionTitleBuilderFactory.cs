namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;

    public interface ActionTitleBuilderFactory
    {
        IActionTitleBuilder GetActionTitleBuilder(int actionId);
    }

    public class ActionTitleBuilderFactoryImpl : ActionTitleBuilderFactory
    {
        private readonly IDictionary<int, IActionTitleBuilder> actionTitleBuilders;

        public ActionTitleBuilderFactoryImpl(IDictionary<int, IActionTitleBuilder> actionTitleBuilders)
        {
            this.actionTitleBuilders = actionTitleBuilders;
        }

        public IActionTitleBuilder GetActionTitleBuilder(int actionId)
            => actionTitleBuilders.ContainsKey(actionId)
                ? actionTitleBuilders[actionId]
                : new DefaultActionTitleBuilder();
    }
}