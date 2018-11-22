namespace Chatbot.Client.Tests.Services
{
    using System;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using NSubstitute;
    using Xunit;

    public class CommandServiceTests
    {
        private readonly IRestClient restClient;
        private readonly CommandServiceImpl commandService;

        public CommandServiceTests()
        {
            restClient = Substitute.For<IRestClient>();

            commandService = new CommandServiceImpl(restClient);
        }

        [Fact]
        public void Run_Alway_ExecuteExecutePostAsync()
        {
            var command = "cm";
            var buttonData = "buttonData";
            var messengerUsername = "username";
            var messengerType = 2;

            commandService.Run(command, buttonData, messengerType, messengerUsername).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualMessengerUsername = type.GetProperty("messengerUsername").GetValue(data);
                var actualMessengerType = (int)type.GetProperty("messengerType").GetValue(data);
                var actualCommand = type.GetProperty("command").GetValue(data);
                var actualButtonData = type.GetProperty("buttonData").GetValue(data);

                return actualMessengerUsername.Equals(messengerUsername)
                    && actualMessengerType == messengerType
                    && actualCommand.Equals(command)
                    && actualButtonData.Equals(buttonData);
            };

            restClient.Received().ExecutePostAsync<Result>(
                "command/run",
                Arg.Is<object>(data => assert(data)));
        }
    }
}