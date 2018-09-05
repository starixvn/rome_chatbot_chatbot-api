namespace Chatbot.Tests.Modules.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    public class FunctionServiceImplTests
    {
        private readonly SettingService subSettingService;
        private readonly IFunctionHandlerFactory subFunctionHandlerFactory;
        private readonly ILogger subLogger;

        public FunctionServiceImplTests()
        {
            subSettingService = Substitute.For<SettingService>();
            subFunctionHandlerFactory = Substitute.For<IFunctionHandlerFactory>();
            subLogger = Substitute.For<ILogger>();
        }

        private FunctionServiceImpl CreateFunctionServiceImpl()
        {
            return new FunctionServiceImpl(
                subSettingService,
                subFunctionHandlerFactory,
                subLogger);
        }

        [Fact]
        public void Handle_UserNotFound_ThrowException()
        {
            // Arrange
            var service = CreateFunctionServiceImpl();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => service.Handle(1, 2).GetAwaiter().GetResult());
        }

        [Fact]
        public void Handle_FunctionNotFound_ThrowException()
        {
            // Arrange
            subSettingService.GetUser(2).Returns(new User());
            var service = CreateFunctionServiceImpl();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => service.Handle(1, 2).GetAwaiter().GetResult());
        }

        [Fact]
        public async Task Handle_Normal_HandleFunctionAndGetResult()
        {
            // Arrange
            var user = new User();
            var messenger = new Messenger();
            var function = new Function { Id = 1 };

            subSettingService.GetUser(2).Returns(user);
            subSettingService.GetMessengers(2, true).Returns(new List<Messenger> { messenger });
            subSettingService.GetFunctions(2).Returns(new List<Function> { function });
            subFunctionHandlerFactory
                .CreateFunctionHandler(1)
                .Handle(Arg.Is<FunctionParam>(fp => fp.User == user && fp.Messenger == messenger && fp.Function == function))
                .Returns(Result.CreateSuccessfulResult());
            var service = CreateFunctionServiceImpl();

            // Act
            var result = await service.Handle(1, 2);

            // Assert
            Assert.True(result.IsOk);
        }
    }
}