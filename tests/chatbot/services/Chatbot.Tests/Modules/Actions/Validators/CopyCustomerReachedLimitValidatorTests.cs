namespace Chatbot.Tests.Modules.Actions.Validators
{
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    public class CopyCustomerReachedLimitValidatorTests
    {
        private readonly CompareLogic compareLogic;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IMessageBuilder messageBuilder;
        private readonly CustomerService customerService;
        private readonly UserLogActionService userLogActionService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly CopyCustomerReachedLimitValidator copyCustomerReachedLimitValidator;

        public CopyCustomerReachedLimitValidatorTests()
        {
            compareLogic = new CompareLogic();
            messageBuilder = Substitute.For<IMessageBuilder>();
            userLogActionService = Substitute.For<UserLogActionService>();
            customerService = Substitute.For<CustomerService>();
            copyCustomerService = Substitute.For<CopyCustomerService>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            copyCustomerReachedLimitValidator = new CopyCustomerReachedLimitValidator(
                messageBuilder,
                customerService,
                userLogActionService,
                copyCustomerService,
                chatbotConfigs);
        }

        [Theory]
        [InlineData(CustLevel.SuperLevel)]
        [InlineData(CustLevel.MasterLevel)]
        public void ValidateAction_CustLevelIsNOTAgent_NotCallsValidateMaxNewMember(byte custLevel)
        {
            // Arrange
            var user = new User(1, 1, custLevel, 0, "en-US", null);
            var messenger = new Messenger(1, "username");
            copyCustomerService.ValidateMaxNewMember(user.Id, user.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result()));

            // Act
            copyCustomerReachedLimitValidator.ValidateAction(user, string.Empty, null, messenger).ConfigureAwait(false);

            // Arrange
            copyCustomerService.DidNotReceive().ValidateMaxNewMember(user.CustId, user.Language);
        }

        [Fact]
        public void ValidateAction_CustLevelIsAgent_CallsValidateMaxNewMember()
        {
            // Arrange
            var messenger = new Messenger(1, "username");
            var user = new User(1, 1, CustLevel.AgentLevel, 0, "en-US", null);
            copyCustomerService.ValidateMaxNewMember(user.Id, user.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result()));

            // Act
            copyCustomerReachedLimitValidator.ValidateAction(user, string.Empty, null, messenger).ConfigureAwait(false);

            // Arrange
            copyCustomerService.Received().ValidateMaxNewMember(user.CustId, user.Language);
        }

        [Fact]
        public void ValidateAction_ValidateMaxNewMemberFailed_ReturnsErrorValidationResult()
        {
            // Arrange
            var messenger = new Messenger(1, "username");
            var user = new User(1, 1, CustLevel.AgentLevel, 0, "en-US", null);
            copyCustomerService.ValidateMaxNewMember(user.Id, user.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = false, Message = "ErrorMessage" }));
            var expectedResult = ActionValidationResult.CreateErrorValidationResult("ErrorMessage", true);

            // Act
            var actualResult = copyCustomerReachedLimitValidator.ValidateAction(user, string.Empty, null, messenger)
                .GetAwaiter().GetResult();

            // Arrange
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Theory]
        [InlineData(CustLevel.SuperLevel)]
        [InlineData(CustLevel.MasterLevel)]
        [InlineData(CustLevel.AgentLevel)]
        public void ValidateAction_Always_CallsIsUserReachedTheLimitOfActionInDay(byte custLevel)
        {
            // Arrange
            var messenger = new Messenger(1, "username");
            var user = new User(1, 1, custLevel, 0, "en-US", null);
            chatbotConfigs.CopyCustomerActionId.Returns(102);
            chatbotConfigs.CopyCustomerLimitPerDay.Returns(50);
            copyCustomerService.ValidateMaxNewMember(user.Id, user.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));

            // Act
            copyCustomerReachedLimitValidator.ValidateAction(user, string.Empty, null, messenger).ConfigureAwait(false);

            // Arrange
            userLogActionService.Received().IsUserReachedTheLimitOfActionInDay(
                user.Id,
                messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay);
        }

        [Theory]
        [InlineData(CustLevel.SuperLevel)]
        [InlineData(CustLevel.MasterLevel)]
        [InlineData(CustLevel.AgentLevel)]
        public void ValidateAction_ReachedLimitOfCopyCustomer_ReturnsErrorValidationResult(byte custLevel)
        {
            // Arrange
            var messenger = new Messenger(1, "username");
            var user = new User(1, 1, custLevel, 0, "en-US", null);
            chatbotConfigs.CopyCustomerActionId.Returns(102);
            chatbotConfigs.CopyCustomerLimitPerDay.Returns(50);

            copyCustomerService.ValidateMaxNewMember(user.Id, user.Language)
                .Returns(Task.FromResult(new Alpha.Services.Client.Models.Result { IsSuccess = true }));
            userLogActionService.IsUserReachedTheLimitOfActionInDay(
                user.Id,
                messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay)
                .Returns(Task.FromResult(true));
            messageBuilder.BuildCopyCustomerReachedLimitMessage(chatbotConfigs.CopyCustomerLimitPerDay, user.Language)
                .Returns("CopyCustomerReachedLimitMessage");

            var expectedResult = ActionValidationResult.CreateErrorValidationResult("CopyCustomerReachedLimitMessage", true);

            // Act
            var actualResult = copyCustomerReachedLimitValidator.ValidateAction(user, string.Empty, null, messenger).Result;

            // Arrange
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}