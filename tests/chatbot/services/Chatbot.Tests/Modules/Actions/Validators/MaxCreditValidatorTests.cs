namespace Chatbot.Tests.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    public class MaxCreditValidatorTests
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CopyCustomerService copyCustomerService;
        private readonly MaxCreditValidator maxCreditValidator;
        private readonly CompareLogic compareLogic;

        public MaxCreditValidatorTests()
        {
            messageBuilder = Substitute.For<IMessageBuilder>();
            copyCustomerService = Substitute.For<CopyCustomerService>();
            maxCreditValidator = new MaxCreditValidator(messageBuilder, copyCustomerService);
            compareLogic = new CompareLogic();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abc")]
        public void ValidateAction_UserResponseIsNOTNumeric_ReturnsErrorValidationResult(string userResponse)
        {
            // Arrange
            var user = new User(1, 1, 3, 0, "en-US", null);
            messageBuilder.BuildInvalidNumericMessage(userResponse, user.Language)
                .Returns("InvalidNumericMessage");
            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidNumericMessage");

            // Act
            var actualResult = maxCreditValidator.ValidateAction(user, userResponse, new List<UserAction>(), null)
                .GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_UserResponseIsNumeric_CallsGetPT()
        {
            // Arrange
            var userResponse = "1234.5";
            var user = new User(1, 1, 3, 0, "en-US", null);
            copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel)
                .Returns(Task.FromResult(new CustomerPTInfo()));

            // Act
            maxCreditValidator.ValidateAction(user, userResponse, new List<UserAction>(), null).ConfigureAwait(false);

            // Assert
            copyCustomerService.Received().GetPT(user.CustId, (byte)user.CustLevel);
        }

        [Fact]
        public void ValidateAction_UserResponseIsNegativeNumber_ReturnsErrorValidationResult()
        {
            // Arrange
            var userResponse = "-1234.5";
            var user = new User(1, 1, 3, 0, "en-US", null);
            var pt = new CustomerPTInfo();
            copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel)
                .Returns(Task.FromResult(pt));
            messageBuilder.BuildInvalidMaxCreditMessage(user.CustLevel - 1, pt.MaxCredit, user.Language)
                .Returns("InvalidMaxCreditMessage");

            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidMaxCreditMessage");

            // Act
            var actualResult = maxCreditValidator.ValidateAction(user, userResponse, new List<UserAction>(), null)
                .GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_UserResponseIsGreaterThanMaxCredit_ReturnsErrorValidationResult()
        {
            // Arrange
            var userResponse = "2";
            var user = new User(1, 1, 3, 0, "en-US", null);
            var pt = new CustomerPTInfo { MaxCredit = 1 };
            copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel)
                .Returns(Task.FromResult(pt));
            messageBuilder.BuildInvalidMaxCreditMessage(user.CustLevel - 1, pt.MaxCredit, user.Language)
                .Returns("InvalidMaxCreditMessage");

            var expectedResult = ActionValidationResult.CreateErrorValidationResult("InvalidMaxCreditMessage");

            // Act
            var actualResult = maxCreditValidator.ValidateAction(user, userResponse, new List<UserAction>(), null)
                .GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_UserResponseIsValid_ReturnsSuccessfulValidationResult()
        {
            // Arrange
            var userResponse = "2";
            var user = new User(1, 1, 3, 0, "en-US", null);
            var pt = new CustomerPTInfo { MaxCredit = 3 };
            copyCustomerService.GetPT(user.CustId, (byte)user.CustLevel)
                .Returns(Task.FromResult(pt));

            var expectedResult = ActionValidationResult.CreateSuccessfulValidationResult();

            // Act
            var actualResult = maxCreditValidator.ValidateAction(user, userResponse, new List<UserAction>(), null)
                .GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}