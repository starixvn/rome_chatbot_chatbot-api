namespace Chatbot.Tests.Modules.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Constants;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;
    using MessengerClient = Messenger.Client;

    [Trait("Modules", "Functions")]
    public class CommandServiceTests
    {
        private readonly SettingService settingService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IFunctionHandlerFactory functionHandlerFactory;
        private readonly ILogger logger;
        private readonly ActionService actionService;

        private readonly CommandService commandService;

        public CommandServiceTests()
        {
            settingService = Substitute.For<SettingService>();
            messageSender = Substitute.For<IMessageSender>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            messageLogger = Substitute.For<IMessageLogger>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            actionService = Substitute.For<ActionService>();
            functionHandlerFactory = Substitute.For<IFunctionHandlerFactory>();
            logger = Substitute.For<ILogger>();

            commandService = new CommandServiceImpl(
                settingService,
                messageSender,
                messageBuilder,
                messageLogger,
                chatbotConfigs,
                functionHandlerFactory,
                logger,
                actionService);
        }

        [Fact]
        public void Execute_MessengerDoesExist_CallLogInfo()
        {
            commandService
                    .Execute("cm", "parameter", 1, "username")
                    .GetAwaiter()
                    .GetResult();

            logger.Received().Info($"messenger username is not verified or enabled");
        }

        [Fact]
        public void Execute_MessengerIsDisabled_ThrowInvalidOperationException()
        {
            var messengerType = 1;
            var messengerUsername = "mess_name";

            settingService
                .GetMessenger(messengerType, messengerUsername)
                .Returns(Task.FromResult(new MessengerDto(1, 1, "name", false, false, "code", DateTime.Now)));

            commandService
                    .Execute("cm", "parameter", messengerType, messengerUsername)
                    .GetAwaiter()
                    .GetResult();

            logger.Received().Info($"messenger mess_name is not verified or enabled");
        }

        [Fact]
        public void Execute_MessengerIsBeingVerify_CallLogInfo()
        {
            var messengerType = 1;
            var messengerUsername = "mess_name";

            settingService
                .GetMessenger(messengerType, messengerUsername)
                .Returns(Task.FromResult(new MessengerDto(1, 1, "name", false, true, "code", DateTime.Now)));

            commandService
                   .Execute("cm", "parameter", messengerType, messengerUsername)
                   .GetAwaiter()
                   .GetResult();

            logger.Received().Info($"messenger mess_name is not verified or enabled");
        }

        [Fact]
        public void Execute_FunctionIsDisabled_ExecuteSendNotRecognizedCommandMessage1()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);
            settingService.GetFunctions(userId).Returns(Task.FromResult(Enumerable.Empty<Function>()));

            commandService
                    .Execute("cm2", "parameter", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageSender
                .Received()
                .SendMessage(
                userId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "NotRecognizedCommandMessage",
                100);
        }

        [Fact]
        public void Execute_Always_LogMessage()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);

            commandService
                    .Execute("cm1", "buttonData", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageLogger
                .Received()
                .LogMessage(
                user.Id,
                CommonId.BotUserId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "cm1",
                0,
                1);
        }

        [Fact]
        public void Execute_Always_ExecuteHandleFunction()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);

            commandService
                    .Execute("cm1", "parameter", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            functionHandlerFactory
                .Received().CreateFunctionHandler(1)
                .Handle(
                    Arg.Is<FunctionParam>(function =>
                        function.User.Id == userId
                        && function.Function.Id == 1
                        && function.Messenger.Username == messenger.Username
                        && function.Others["command"].Equals("cm1")));
        }

        [Fact]
        public void Execute_NoRootActionExist_ExecuteSendNotRecognizedCommandMessage()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);
            StubFunctions();
            actionService.NoRootActionExist().Returns(Task.FromResult(true));

            commandService
                    .Execute("cm", "parameter", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageSender
                .Received()
                .SendMessage(
                userId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "NotRecognizedCommandMessage",
                100);
        }

        [Fact]
        public void Execute_AvtionValidationIsFailed_SendValidationMessage()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);
            StubFunctions();
            actionService
                .BuildAction("cm", "buttondata", Arg.Any<User>(), Arg.Any<Messenger>())
                .Returns(Task.FromResult(
                    new ActionResult(new MessengerClient.Models.Message(), "Failed Validation Message", 1)));

            commandService
                    .Execute("cm", "buttondata", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageSender
                .Received()
                .SendMessage(
                userId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                "Failed Validation Message",
                1);
        }

        [Fact]
        public void Execute_IsNotEndOfActionChain_SendNextStepMessage()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);
            StubFunctions();
            var nextStep = new MessengerClient.Models.Message { Content = "Next step" };
            actionService
                .BuildAction("cm", "buttondata", Arg.Any<User>(), Arg.Any<Messenger>())
                .Returns(Task.FromResult(
                    new ActionResult(nextStep, 2)));

            commandService
                    .Execute("cm", "buttondata", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            messageSender
                .Received()
                .SendMessage(
                userId,
                Arg.Is<Messenger>(
                    msg => msg.Type == messenger.Type
                        && msg.Username == messenger.Username),
                nextStep,
                2);
        }

        [Fact]
        public void Execute_ActionIsEndChain_ExecuteHandleCommand()
        {
            var userId = 190;
            var messenger = new Messenger(1, "messengerName");
            var user = StubUser(userId);

            SetupSendCommand(userId, messenger, user);
            StubFunctions();
            actionService
                .BuildAction("cm1", "buttondata", Arg.Any<User>(), Arg.Any<Messenger>())
                .Returns(Task.FromResult(
                    new ActionResult("cm1", 1)));

            commandService
                    .Execute("cm1", "buttondata", messenger.Type, messenger.Username)
                    .GetAwaiter()
                    .GetResult();

            functionHandlerFactory
                .Received().CreateFunctionHandler(1)
                .Handle(
                    Arg.Is<FunctionParam>(function =>
                        function.User.Id == userId
                        && function.Function.Id == 1
                        && function.Messenger.Username == messenger.Username
                        && function.Others["command"].Equals("cm1")));
        }

        private void SetupSendCommand(int userId, Messenger messenger, User user)
        {
            settingService.GetFunctions(userId).Returns(Task.FromResult(StubFunctions()));
            settingService
                .GetMessenger(messenger.Type, messenger.Username)
                .Returns(Task.FromResult(new MessengerDto(userId, 1, "name", true, false, "code", DateTime.Now)));
            settingService.GetUser(userId).Returns(user);
            chatbotConfigs.NotRecognizedCommandFunctionId.Returns(100);
            messageBuilder.BuildNotRecognizedCommandMessage(user.Language).Returns("NotRecognizedCommandMessage");
        }

        private User StubUser(int userId)
            => new User(
                userId,
                userId + 1,
                3,
                userId + 100,
                "en-US",
                Enumerable.Empty<Messenger>(),
                Enumerable.Empty<Function>());

        private IEnumerable<Function> StubFunctions()
            => new List<Function>
            {
                new Function
                {
                    Id = 1,
                    Command = "cm1",
                    Type = 1,
                    Enabled = true
                },
                new Function
                {
                    Id = 2,
                    Type = 1,
                    Command = "cm2",
                    Enabled = false
                }
            };
    }
}