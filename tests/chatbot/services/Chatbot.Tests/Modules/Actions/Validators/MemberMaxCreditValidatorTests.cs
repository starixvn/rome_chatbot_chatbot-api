namespace Chatbot.Tests.Modules.Actions.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    public class MemberMaxCreditValidatorTests
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CompareLogic compareLogic;
        private readonly MemberMaxCreditValidator memberMaxCreditValidator;

        public MemberMaxCreditValidatorTests()
        {
            messageBuilder = Substitute.For<IMessageBuilder>();
            compareLogic = new CompareLogic();
            memberMaxCreditValidator = new MemberMaxCreditValidator(messageBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("aaa")]
        public async Task ValidateAction_UserResponseIsNotNumeric_ReturnsErrorValidationResult(string userResponse)
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", null);
            messageBuilder.BuildInvalidNumericMessage(userResponse, user.Language)
                .Returns("InvalidNumericMessage");
            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidNumericMessage");

            // Act
            var actualResult = await memberMaxCreditValidator.ValidateAction(user, userResponse, null, null);

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public async Task ValidateAction_UserResponseIsNumericAndLessThanZero_ReturnsErrorValidationResult()
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", 15);
            messageBuilder.BuildInvalidMaxCreditMessage(CustLevel.MemberLevel, Arg.Any<decimal>(), user.Language)
                .Returns("InvalidMaxMemberCreditMessage");
            var userActions = new List<UserAction> {
                new UserAction(1,1,14,"1000",DateTime.Now)
            };
            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidMaxMemberCreditMessage");

            // Act
            var actualResult = await memberMaxCreditValidator.ValidateAction(user, "-1", userActions, null);

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public async Task ValidateAction_UserResponseIsNumericAndGreaterThanPreviousResponse_ReturnsErrorValidationResult()
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", 15);
            messageBuilder.BuildInvalidMaxCreditMessage(CustLevel.MemberLevel, Arg.Any<decimal>(), user.Language)
                .Returns("InvalidMaxMemberCreditMessage");
            var userActions = new List<UserAction> {
                new UserAction(1,1,14,"1000",DateTime.Now)
            };
            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidMaxMemberCreditMessage");

            // Act
            var actualResult = await memberMaxCreditValidator.ValidateAction(user, "1001", userActions, null);

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public async Task ValidateAction_UserResponseIsValid_ReturnsErrorValidationResult()
        {
            // Arrange
            var user = new User(1, 1, 2, 0, "en-US", 15);
            var userActions = new List<UserAction> {
                new UserAction(1,1,14,"1000",DateTime.Now)
            };
            var expectedResult = ActionValidationResult.CreateSuccessfulValidationResult();

            // Act
            var actualResult = await memberMaxCreditValidator.ValidateAction(user, "1", userActions, null);

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}