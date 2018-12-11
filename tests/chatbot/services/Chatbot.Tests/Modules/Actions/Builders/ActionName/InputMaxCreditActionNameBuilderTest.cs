namespace Chatbot.Tests.Modules.Actions.Builders.ActionName
{
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Modules.Resources.Models;
    using NSubstitute;
    using Xunit;

    public class InputMaxCreditActionNameBuilderTest
    {
        private readonly ResourceService resourceService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly InputMaxCreditActionNameBuilder inputMaxCreditActionNameBuilder;

        public InputMaxCreditActionNameBuilderTest()
        {
            resourceService = Substitute.For<ResourceService>();
            copyCustomerService = Substitute.For<CopyCustomerService>();
            inputMaxCreditActionNameBuilder = new InputMaxCreditActionNameBuilder(
                resourceService,
                copyCustomerService);
        }

        [Fact]
        public void BuildActionName_Always_CallsGetPT()
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", 15);
            resourceService.GetResource(1, ResourceType.Action, user.Language)
                .Returns(Task.FromResult(new Resource(1, ResourceType.Action, "en-US", "Action name")));

            // Act
            inputMaxCreditActionNameBuilder.BuildActionName(1, user, null).ConfigureAwait(false);

            // Assert
            copyCustomerService.Received().GetPT(user.CustId, (byte)user.CustLevel);
        }

        [Fact]
        public void BuildActionName_Always_CallsGetResource()
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", 15);
            resourceService.GetResource(1, ResourceType.Action, user.Language)
                .Returns(Task.FromResult(new Resource(1, ResourceType.Action, "en-US", "Action name")));

            // Act
            inputMaxCreditActionNameBuilder.BuildActionName(1, user, null).ConfigureAwait(false);

            // Assert
            resourceService.Received().GetResource(1, ResourceType.Action, user.Language);
        }
    }
}