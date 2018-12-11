namespace Chatbot.Tests.Modules.Actions.Builders.ActionName
{
    using System.Collections.Generic;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Shared.Configurations;
    using NSubstitute;
    using Xunit;

    public class InputMemberMaxCreditActionNameBuilderTests
    {
        private readonly ResourceService resourceService;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly InputMemberMaxCreditActionNameBuilder inputMemberMaxCreditActionNameBuilder;

        public InputMemberMaxCreditActionNameBuilderTests()
        {
            resourceService = Substitute.For<ResourceService>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            inputMemberMaxCreditActionNameBuilder = new InputMemberMaxCreditActionNameBuilder(
                resourceService,
                chatbotConfigs);
        }

        [Fact]
        public void BuildActionName_Always_CallsGetResource()
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", null);

            // Act
            inputMemberMaxCreditActionNameBuilder.BuildActionName(1, user, new List<UserAction>()).ConfigureAwait(false);

            // Assert
            resourceService.Received().GetResource(1, ResourceType.Action, user.Language);
        }
    }
}