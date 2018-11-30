namespace Chatbot.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;

    public class DefaultActionTitleBuilder : IActionTitleBuilder
    {
        private readonly IMessageBuilder messageBuilder;

        public DefaultActionTitleBuilder(IMessageBuilder messageBuilder)
        {
            this.messageBuilder = messageBuilder;
        }

        public string BuildActionTitle(User user, IList<UserAction> userActions)
            => messageBuilder.BuildActionTitle(user.Language);
    }
}