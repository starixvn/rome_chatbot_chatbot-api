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
            var parameter = "parameter";
            var messengerUsername = "username";
            var messengerType = 2;

            commandService.Run(command, parameter, messengerType, messengerUsername).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualMessengerUsername = type.GetProperty("messengerUsername").GetValue(data);
                var actualMessengerType = (int)type.GetProperty("messengerType").GetValue(data);
                var actualCommand = type.GetProperty("command").GetValue(data);
                var actualParameter = type.GetProperty("parameter").GetValue(data);

                return actualMessengerUsername == messengerUsername
                    && actualMessengerType == messengerType
                    && actualCommand == command
                    && actualParameter == parameter;
            };

            restClient.Received().ExecutePostAsync<Result>(
                "command/run",
                Arg.Is<object>(data => assert(data)));
        }
    }
}