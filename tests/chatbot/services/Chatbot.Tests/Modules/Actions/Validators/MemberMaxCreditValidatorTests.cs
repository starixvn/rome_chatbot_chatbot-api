namespace Chatbot.Tests.Modules.Actions.Validators
{
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Shared.Configurations;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    public class MemberMaxCreditValidatorTests
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly CompareLogic compareLogic;
        private readonly MemberMaxCreditValidator memberMaxCreditValidator;

        public MemberMaxCreditValidatorTests()
        {
            messageBuilder = Substitute.For<IMessageBuilder>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            compareLogic = new CompareLogic();
            memberMaxCreditValidator = new MemberMaxCreditValidator(messageBuilder, chatbotConfigs);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("aaa")]
        public void ValidateAction_UserResponseIsNotNumeric_ReturnsErrorValidationResult(string userResponse)
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", null);
            messageBuilder.BuildInvalidNumericMessage(userResponse, user.Language)
                .Returns("InvalidNumericMessage");
            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidNumericMessage");

            // Act
            var actualResult = memberMaxCreditValidator.ValidateAction(user, userResponse, null, null).Result;

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}