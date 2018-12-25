using System.Collections.Generic;
using System.Threading.Tasks;
using Chatbot.Models;
using Chatbot.Modules.Actions.Builders.ActionName;
using Chatbot.Modules.Actions.Models;
using Chatbot.Modules.Resources;
using Chatbot.Modules.Resources.Constants;
using Chatbot.Modules.Resources.Models;
using NSubstitute;
using Xunit;

namespace Chatbot.Tests.Modules.Actions.Builders.ActionName
{
    public class DefaultActionNameBuilderTests
    {
        private readonly ResourceService resourceService;
        private readonly DefaultActionNameBuilder defaultActionNameBuilder;

        public DefaultActionNameBuilderTests()
        {
            resourceService = Substitute.For<ResourceService>();
            defaultActionNameBuilder = new DefaultActionNameBuilder(resourceService);
        }

        [Fact]
        public void BuildActionName_Always_CallsGetResource()
        {
            // Arrange
            var actionId = 1;
            var user = new User(1, 1, 2, 0, "en-US", null);
            var resource = new Resource(actionId, ResourceType.Action, "en-US", "");
            resourceService.GetResource(actionId, ResourceType.Action, user.Language)
                .Returns(Task.FromResult(resource));

            // Act
            defaultActionNameBuilder.BuildActionName(actionId, user, new List<UserAction>()).ConfigureAwait(false);

            // Assert
            resourceService.Received().GetResource(actionId, ResourceType.Action, user.Language);
        }
    }
}