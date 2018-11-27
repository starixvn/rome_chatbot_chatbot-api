namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using Chatbot.Modules.MessageBuilders;

    public interface ActionTitleBuilderFactory
    {
        IActionTitleBuilder GetActionTitleBuilder(int actionId);
    }

    public class ActionTitleBuilderFactoryImpl : ActionTitleBuilderFactory
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IDictionary<int, IActionTitleBuilder> actionTitleBuilders;

        public ActionTitleBuilderFactoryImpl(
            IMessageBuilder messageBuilder,
            IDictionary<int, IActionTitleBuilder> actionTitleBuilders)
        {
            this.messageBuilder = messageBuilder;
            this.actionTitleBuilders = actionTitleBuilders;
        }

        public IActionTitleBuilder GetActionTitleBuilder(int actionId)
            => actionTitleBuilders.ContainsKey(actionId)
                ? actionTitleBuilders[actionId]
                : new DefaultActionTitleBuilder(messageBuilder);
    }
}