namespace Chatbot.Tests.Modules.Actions.Validators
{
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    public class NumberValueActionValidatorTests
    {
        private readonly CompareLogic compareLogic;
        private readonly IMessageBuilder messageBuilder;
        private readonly NumberValueActionValidator validator;

        public NumberValueActionValidatorTests()
        {
            compareLogic = new CompareLogic();
            messageBuilder = Substitute.For<IMessageBuilder>();
            validator = new NumberValueActionValidator(messageBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        [InlineData("12.45")]
        [InlineData("3000000000")]
        public void ValidateAction_UserResponseIsNullOrWhiteSpaceOrNotIntegerNumber_ReturnsErrorResult(string userResponse)
        {
            // Arrange
            var invalidNumerOfDownlineMessage = "Invalid number of downline";
            var expectedResult = Result.CreateErrorResult(invalidNumerOfDownlineMessage);
            messageBuilder.BuildInvalidNumerOfDownline(Arg.Any<string>()).Returns(invalidNumerOfDownlineMessage);

            // Act
            var actualResult = validator.ValidateAction(1, new User(), userResponse);

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("1")]
        [InlineData("1000000")]
        public void ValidateAction_UserResponseIsValid_ReturnsSuccessfulResult(string userResponse)
        {
            // Arrange
            var expectedResult = Result.CreateSuccessfulResult();

            // Act
            var actualResult = validator.ValidateAction(1, new User(), userResponse);

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}