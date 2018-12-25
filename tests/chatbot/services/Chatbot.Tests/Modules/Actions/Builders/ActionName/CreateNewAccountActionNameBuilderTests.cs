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

    public class CreateNewAccountActionNameBuilderTests
    {
        private readonly ResourceService resourceService;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly CreateNewAccountActionNameBuilder createNewAccountActionNameBuilder;

        public CreateNewAccountActionNameBuilderTests()
        {
            resourceService = Substitute.For<ResourceService>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();

            createNewAccountActionNameBuilder = new CreateNewAccountActionNameBuilder(
                resourceService,
                chatbotConfigs);
        }

        [Theory]
        [InlineData(CustLevel.AgentLevel)]
        [InlineData(CustLevel.MasterLevel)]
        [InlineData(CustLevel.SuperLevel)]
        public void BuildActionName_Always_CallsGetResource(byte custLevel)
        {
            var actionId = 1;
            var user = new User(1, 1, custLevel, 0, "en-US", null);
            resourceService.GetResource(1, ResourceType.Action, user.Language)
                .Returns(Task.FromResult(new Resource(1, ResourceType.Action, "en-US", "Action name")));

            var userActions = new List<UserAction>
            {
                new UserAction(1,1,0,"1000",DateTime.Now,1)
            };

            // Act
            createNewAccountActionNameBuilder.BuildActionName(actionId, user, userActions).ConfigureAwait(false);

            // Assert
            resourceService.Received().GetResource(actionId, ResourceType.Action, user.Language);
        }
    }
}