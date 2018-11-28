namespace Chatbot.Tests.Modules.Functions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.Settings;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "Functions")]
    public class FunctionServiceImplTests
    {
        private readonly SettingService subSettingService;
        private readonly IFunctionHandlerFactory subFunctionHandlerFactory;

        public FunctionServiceImplTests()
        {
            subSettingService = Substitute.For<SettingService>();
            subFunctionHandlerFactory = Substitute.For<IFunctionHandlerFactory>();
        }

        private FunctionServiceImpl CreateFunctionServiceImpl()
        {
            return new FunctionServiceImpl(
                subSettingService,
                subFunctionHandlerFactory);
        }

        [Fact]
        public async Task GetFunctionMessages_Always_GetExpectedFunctionMessages()
        {
            // Arrange
            var users = new List<User> {
                    new User(1, 2, 3, 4, "en-US", null)
            };
            var expectedFunctionMessages = new List<FunctionMessage>
            {
                new FunctionMessage(1, "message", null)
            };
            subSettingService.GetUsersByFunction(2, 0).Returns(users);
            subFunctionHandlerFactory
                .CreateFunctionHandler(2)
                .GetFunctionMessages(Arg.Any<FunctionParam>())
                .Returns(expectedFunctionMessages);
            var functionService = CreateFunctionServiceImpl();

            // Act
            var functionMessages = await functionService.GetFunctionMessages(2);

            // Assert
            Assert.Equal(expectedFunctionMessages, functionMessages.ToList());
            subFunctionHandlerFactory
                .Received().CreateFunctionHandler(Arg.Is(2));
        }
    }
}