namespace Chatbot.WebApi.Tests.Controllers
{
    using Chatbot.Modules.Funtions;
    using Chatbot.WebApi.Controllers;
    using Chatbot.WebApi.Params;
    using NSubstitute;
    using Xunit;

    public class CommandControllerTests
    {
        private readonly CommandService commandService;
        private readonly CommandController commandController;

        public CommandControllerTests()
        {
            commandService = Substitute.For<CommandService>();
            commandController = new CommandController(commandService);
        }

        [Fact]
        public void Run_Always_CallsCommandServiceExcute()
        {
            // Arrange
            var param = new CommandParam
            {
                Command = "command",
                ButtonData = "parameter",
                MessengerType = 1,
                MessengerUsername = "username"
            };

            // Act
            commandController.Run(param).ConfigureAwait(false);

            // Arrange
            commandService.Received().Execute(
                param.Command,
                param.ButtonData,
                param.MessengerType,
                param.MessengerUsername);
        }
    }
}