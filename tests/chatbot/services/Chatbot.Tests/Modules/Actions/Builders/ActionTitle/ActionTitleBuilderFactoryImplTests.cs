namespace Chatbot.Tests.Modules.Actions.Builders.ActionTitle
{
    using System.Collections.Generic;
    using Chatbot.Modules.Actions.Builders.ActionTitle;
    using Xunit;

    public class ActionTitleBuilderFactoryImplTests
    {
        private IDictionary<int, IActionTitleBuilder> actionTitleBuilders;
        private ActionTitleBuilderFactoryImpl actionTitleBuilderFactory;

        [Fact]
        public void GetActionTitleBuilder_NOTContainsActionId_ReturnsDefaultActionTitleBuilder()
        {
            // Arrange
            actionTitleBuilders = new Dictionary<int, IActionTitleBuilder>();
            actionTitleBuilderFactory = new ActionTitleBuilderFactoryImpl(actionTitleBuilders);

            // Act
            var result = actionTitleBuilderFactory.GetActionTitleBuilder(1);

            // Assert
            Assert.True(result is DefaultActionTitleBuilder);
        }
    }
}