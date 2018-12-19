namespace Chatbot.Tests.Modules.Actions.Builders.ActionName
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Modules.Resources.Models;
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
            chatbotConfigs.CopyAgentInputCreditActionId.Returns(2);
            resourceService.GetResource(Arg.Any<int>(), ResourceType.Action, user.Language)
                .Returns(Task.FromResult(new Resource(1, ResourceType.Action, user.Language, "value")));
            var userActions = new List<UserAction>
            {
                new UserAction(1,1,2,"1000",DateTime.Now,1)
            };

            // Act
            inputMemberMaxCreditActionNameBuilder.BuildActionName(1, user, userActions).ConfigureAwait(false);

            // Assert
            resourceService.Received().GetResource(1, ResourceType.Action, user.Language);
        }
    }
}