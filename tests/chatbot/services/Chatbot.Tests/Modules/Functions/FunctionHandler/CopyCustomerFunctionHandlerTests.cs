namespace Chatbot.Tests.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Functions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageLoggers;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;
    using Fanex.Logging;
    using NSubstitute;
    using Xunit;

    public class CopyCustomerFunctionHandlerTests
    {
        private readonly ILogger logger;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly UserLogActionService userLogActionService;
        private readonly CustomerService customerService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly IMessageLogger messageLogger;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;

        private readonly CopyCustomerFunctionHandler copyCustomerFunctionHandler;

        public CopyCustomerFunctionHandlerTests()
        {
            logger = Substitute.For<ILogger>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            messageSender = Substitute.For<IMessageSender>();
            userLogActionService = Substitute.For<UserLogActionService>();
            customerService = Substitute.For<CustomerService>();
            copyCustomerService = Substitute.For<CopyCustomerService>();
            messageLogger = Substitute.For<IMessageLogger>();
            getDataHandlers = new Dictionary<int, Func<FunctionParam, Task<object>>>();

            copyCustomerFunctionHandler = new CopyCustomerFunctionHandler(
                chatbotConfigs,
                messageBuilder,
                messageSender,
                userLogActionService,
                customerService,
                copyCustomerService,
                logger,
                messageLogger,
                getDataHandlers);

            chatbotConfigs.CopyCustomerActionId.Returns(102);
            chatbotConfigs.CopyCustomerLimitPerDay.Returns(50);
        }

        [Fact]
        public void RunHandler_Always_CallsGetCustomer()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            customerService.Received().GetCustomer(param.User.CustId);
        }

        [Fact]
        public void RunHandler_NotEnoughParameters_SendCopyCustomerInstructionMessage()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            customerService.GetCustomer(param.User.CustId).Returns(Task.FromResult(new CustomerInfo()));
            messageBuilder.BuildCopyCustomerInstructionMessage(
                Arg.Any<string>(),
                param.User.CustLevel - 1,
                param.Messenger.Type,
                param.User.Language).Returns("CopyCustomerInstructionMessage");

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "CopyCustomerInstructionMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void RunHandler_SourceUserNameIsNullOrWhiteSpace_SendInvalidDirectDownlineMessage(string username)
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { username, "" };
            messageBuilder.BuildInvalidDirectDownlineMessage(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<string>())
                .Returns("InvalidDirectDownlineMessage");

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "InvalidDirectDownlineMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        public void RunHandler_CreditParamIsNotNumeric_SendInvalidNumericMessage(string credit)
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", credit };
            messageBuilder.BuildInvalidNumericMessage(Arg.Any<string>(), Arg.Any<string>())
                .Returns("InvalidNumericMessage");

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "InvalidNumericMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        public void RunHandler_GivenMemberCreditParamIsNotNumeric_SendInvalidNumericMessage(string credit)
        {
            // Arrange
            var user = new User(1, 1, 3, 0, "en-US", null);
            var messenger = new Messenger(1, "username");
            var function = new Function() { Id = 9 };
            var others = new Dictionary<string, object>
            {
                ["parameters"] = new string[] { "username", "123", credit }
            };

            var param = new FunctionParam(user, function, messenger, others);
            messageBuilder.BuildInvalidNumericMessage(Arg.Any<string>(), Arg.Any<string>())
                .Returns("InvalidNumericMessage");

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "InvalidNumericMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Fact]
        public void RunHandler_ValidateReachedLitmitOfCopyCustomer_CustLevelIsAgent_CallsValidateMaxNewMember()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            copyCustomerService.Received().ValidateMaxNewMember(param.User.CustId, param.User.Language);
        }

        [Theory]
        [InlineData(CustLevel.SuperLevel)]
        [InlineData(CustLevel.MasterLevel)]
        public void RunHandler_ValidateReachedLitmitOfCopyCustomer_CustLevelIsNOTAgent_CallsValidateMaxNewMember(byte custLevel)
        {
            // Arrange
            var user = new User(1, 1, custLevel, 0, "en-US", null);
            var messenger = new Messenger(1, "username");
            var function = new Function() { Id = 9 };
            var others = new Dictionary<string, object>
            {
                ["parameters"] = new string[] { "username", "123" }
            };
            var param = new FunctionParam(user, function, messenger, others);

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            copyCustomerService.DidNotReceive().ValidateMaxNewMember(param.User.CustId, param.User.Language);
        }

        [Fact]
        public void RunHandler_ValidateReachedLitmitOfCopyCustomer_ValidateMaxNewMemberFailed_SendErrorMessage()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = false, Message = "ErrorMessage" }));
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "ErrorMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Fact]
        public void RunHandler_ValidateReachedLitmitOfCopyCustomer_Always_CheckUserReachedTheLimitOfActionInDay()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            userLogActionService.Received().IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay);
        }

        [Fact]
        public void RunHandler_ReachedLimitOfCopyCustomer_SendCopyCustomerReachedLimitMessage()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(true);
            messageBuilder.BuildCopyCustomerReachedLimitMessage(Arg.Any<int>(), Arg.Any<string>())
                .Returns("CopyCustomerReachedLimitMessage");
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "CopyCustomerReachedLimitMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Fact]
        public void RunHandler_ValidateDownline_Always_GetDownlineInfo()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(false);
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });
            copyCustomerService.CheckReachLimitDownline(param.User.CustId, (byte)param.User.CustLevel, "username", param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            copyCustomerService.CheckLimitNewDownline(param.User.CustId, (byte)param.User.CustLevel, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            customerService.Received().GetDownlineInfo(param.User.CustId, "username");
        }

        [Fact]
        public void RunHandler_ValidateDownline_IsNotDirectDownline_SendInvalidDirectDownlineMessage()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(false);
            customerService.GetDownlineInfo(param.User.CustId, "username")
                .Returns(Task.FromResult(new DownlineInfo { RoleId = 0 }));
            messageBuilder.BuildInvalidDirectDownlineMessage(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>())
                .Returns("InvalidDirectDownlineMessage");
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });
            copyCustomerService.CheckReachLimitDownline(param.User.CustId, (byte)param.User.CustLevel, "username", param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            copyCustomerService.CheckLimitNewDownline(param.User.CustId, (byte)param.User.CustLevel, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            
            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
                param.User.Id,
                param.Messenger,
                "InvalidDirectDownlineMessage",
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        [Fact]
        public void RunHandler_ValidateMaxCredit_Always_GetPT()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "123" };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(false);
            customerService.GetDownlineInfo(param.User.CustId, "username")
                .Returns(Task.FromResult(new DownlineInfo { CustId = 1, RoleId = (byte)(param.User.CustLevel - 1) }));
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });
            copyCustomerService.CheckReachLimitDownline(param.User.CustId, (byte)param.User.CustLevel, "username", param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            copyCustomerService.CheckLimitNewDownline(param.User.CustId, (byte)param.User.CustLevel, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            copyCustomerService.Received().GetPT(param.User.CustId, (byte)param.User.CustLevel);
        }

        [Theory]
        [InlineData("-1")]
        [InlineData("1100")]
        public void RunHandler_ValidateMaxCredit_InvalidCredit_SendInvalidMaxCreditMessage(string credit)
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", credit };
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(false);
            customerService.GetDownlineInfo(param.User.CustId, "username")
                .Returns(Task.FromResult(new DownlineInfo { CustId = 1, RoleId = (byte)(param.User.CustLevel - 1) }));
            copyCustomerService.GetPT(param.User.CustId, (byte)param.User.CustLevel)
                .Returns(Task.FromResult(new CustomerPTInfo { MaxCredit = 1000 }));
            messageBuilder.BuildInvalidMaxCreditMessage(param.User.CustLevel - 1, 1000, param.User.Language)
                .Returns("InvalidMaxCreditMessage");
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });
            copyCustomerService.CheckReachLimitDownline(param.User.CustId, (byte)param.User.CustLevel, "username", param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            copyCustomerService.CheckLimitNewDownline(param.User.CustId, (byte)param.User.CustLevel, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
               param.User.Id,
               param.Messenger,
               "InvalidMaxCreditMessage",
               param.User.SelfdestructionPeriod,
               param.Function.Id);
        }

        [Fact]
        public void RunHandler_PassedValidation_SendSendCopyCustomerInformMessage()
        {
            // Arrange
            var param = CreateDefaultFunctionParam();
            param.Others["parameters"] = new string[] { "username", "500" };
            customerService.GetCustomer(param.User.CustId)
                .Returns(Task.FromResult(new CustomerInfo { UserName = "username" }));
            copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(false);
            customerService.GetDownlineInfo(param.User.CustId, "username")
                .Returns(Task.FromResult(new DownlineInfo { CustId = 1, RoleId = (byte)(param.User.CustLevel - 1) }));
            copyCustomerService.GetPT(param.User.CustId, (byte)param.User.CustLevel)
                .Returns(Task.FromResult(new CustomerPTInfo { MaxCredit = 1000 }));
            messageBuilder.BuildCopyCustomerInformMessage(param.User.Language)
                .Returns("CopyCustomerInformMessage");
            customerService.GetCustomer(param.User.CustId).Returns(new CustomerInfo { UserName = "username" });
            copyCustomerService.CheckReachLimitDownline(param.User.CustId, (byte)param.User.CustLevel, "username", param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            copyCustomerService.CheckLimitNewDownline(param.User.CustId, (byte)param.User.CustLevel, param.User.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));

            // Act
            copyCustomerFunctionHandler.RunHandler(param).ConfigureAwait(false);

            // Assert
            messageSender.Received().SendMessage(
               param.User.Id,
               param.Messenger,
               "CopyCustomerInformMessage",
               param.User.SelfdestructionPeriod,
               param.Function.Id);
        }

        private static FunctionParam CreateDefaultFunctionParam()
        {
            var user = new User(1, 1, 2, 0, "en-US", null);
            var messenger = new Messenger(1, "username");
            var function = new Function() { Id = 9 };
            var others = new Dictionary<string, object>
            {
                ["parameters"] = new string[] { }
            };

            return new FunctionParam(user, function, messenger, others);
        }
    }
}