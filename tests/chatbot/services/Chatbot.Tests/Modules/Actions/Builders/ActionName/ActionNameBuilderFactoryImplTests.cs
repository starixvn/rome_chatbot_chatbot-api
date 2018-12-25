namespace Chatbot.Tests.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Resources;
    using NSubstitute;
    using Xunit;

    public class ActionNameBuilderFactoryImplTests
    {
        private readonly ResourceService resourceService;
        private IDictionary<int, IActionNameBuilder> actionNameBuilders;
        private ActionNameBuilderFactoryImpl actionNameBuilderFactory;

        public ActionNameBuilderFactoryImplTests()
        {
            resourceService = Substitute.For<ResourceService>();
        }

        [Fact]
        public void GetActionNameBuilder_NOTContainsActionId_ReturnsDefaultActionNameBuilder()
        {
            // Arrange
            actionNameBuilders = new Dictionary<int, IActionNameBuilder>();
            actionNameBuilderFactory = new ActionNameBuilderFactoryImpl(resourceService, actionNameBuilders);

            // Act
            var result = actionNameBuilderFactory.GetActionNameBuilder(1);

            // Assert
            Assert.True(result is DefaultActionNameBuilder);
        }
    }
}