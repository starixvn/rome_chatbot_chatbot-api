namespace Chatbot.Tests.Modules.Actions.Validators
{
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

    public class DownlineActionValidatorTests
    {
        private readonly CompareLogic compareLogic;
        private readonly IMessageBuilder messageBuilder;
        private readonly CustomerService customerService;
        private readonly DownlineActionValidator validator;

        public DownlineActionValidatorTests()
        {
            compareLogic = new CompareLogic();
            messageBuilder = Substitute.For<IMessageBuilder>();
            customerService = Substitute.For<CustomerService>();

            validator = new DownlineActionValidator(messageBuilder, customerService);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateAction_UserResponseIsNullOrWhiteSpace_ReturnsErrorResult(string userResponse)
        {
            // Arrange
            var invalidDownlineMessage = "Invalid downline";
            var expectedResult = ActionValidationResult.CreateErrorValidationResult(invalidDownlineMessage);
            messageBuilder.BuildInvalidDownlineMessage(Arg.Any<string>(), Arg.Any<string>()).Returns(invalidDownlineMessage);

            // Act
            var actualResult = validator.ValidateAction(new User(), userResponse, null, null).GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_ValidDownline_ReturnsSuccessfulResult()
        {
            // Arrange
            var downlineInfo = new DownlineInfo
            {
                CustId = 1,
                IsActive = true,
                IsSubAccount = false,
                RoleId = 2
            };
            customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
                .Returns(Task.FromResult(downlineInfo));

            var expectedResult = ActionValidationResult.CreateSuccessfulValidationResult();

            // Act
            var actualResult = validator.ValidateAction(new User(), "abc", null, null).GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_InvalidDownline_ReturnsErrorResult()
        {
            // Arrange
            var downlineInfo = new DownlineInfo();
            customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
                .Returns(Task.FromResult(downlineInfo));

            var invalidDownlineMessage = "Invalid downline";
            messageBuilder.BuildInvalidDownlineMessage(Arg.Any<string>(), Arg.Any<string>()).Returns(invalidDownlineMessage);

            var expectedResult = ActionValidationResult.CreateErrorValidationResult(invalidDownlineMessage);

            // Act
            var actualResult = validator.ValidateAction(new User(), "abc", null, null).GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}