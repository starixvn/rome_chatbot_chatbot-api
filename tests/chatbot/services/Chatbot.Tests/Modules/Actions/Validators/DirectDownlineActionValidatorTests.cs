namespace Chatbot.Tests.Modules.Actions.Validators
{
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    public class DirectDownlineActionValidatorTests
    {
        private readonly CompareLogic compareLogic;
        private readonly IMessageBuilder messageBuilder;
        private readonly CustomerService customerService;
        private readonly DirectDownlineActionValidator validator;

        public DirectDownlineActionValidatorTests()
        {
            compareLogic = new CompareLogic();
            messageBuilder = Substitute.For<IMessageBuilder>();
            customerService = Substitute.For<CustomerService>();

            validator = new DirectDownlineActionValidator(messageBuilder, customerService);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateAction_UserResponseIsNullOrWhiteSpace_ReturnsErrorResult(string userResponse)
        {
            // Arrange
            var invalidDirectDownlineMessage = "Invalid direct downline";
            var expectedResult = Result.CreateErrorResult(invalidDirectDownlineMessage);
            messageBuilder.BuildInvalidDirectDownlineMessage(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>())
                .Returns(invalidDirectDownlineMessage);

            // Act
            var actualResult = validator.ValidateAction(1, new User(), userResponse);

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

            var invalidDirectDownlineMessage = "Invalid direct downline";
            messageBuilder.BuildInvalidDirectDownlineMessage(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>())
                .Returns(invalidDirectDownlineMessage);

            var expectedResult = Result.CreateErrorResult(invalidDirectDownlineMessage);

            // Act
            var actualResult = validator.ValidateAction(1, new User(), "abc");

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_IsNotDirectDownline_ReturnsErrorResult()
        {
            // Arrange
            var user = new User(1, 2, 4, 0, "en-US");
            var downlineInfo = new DownlineInfo
            {
                CustId = 1,
                IsActive = true,
                IsSubAccount = false,
                RoleId = 2
            };
            customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
                .Returns(Task.FromResult(downlineInfo));

            var invalidDirectDownlineMessage = "Invalid direct downline";
            messageBuilder.BuildInvalidDirectDownlineMessage(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<string>())
                .Returns(invalidDirectDownlineMessage);

            var expectedResult = Result.CreateErrorResult(invalidDirectDownlineMessage);

            // Act
            var actualResult = validator.ValidateAction(1, user, "abc");

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void ValidateAction_ValidDirectDownline_ReturnsSuResult()
        {
            // Arrange
            var user = new User(1, 2, 4, 0, "en-US");
            var downlineInfo = new DownlineInfo
            {
                CustId = 1,
                IsActive = true,
                IsSubAccount = false,
                RoleId = 3
            };
            customerService.GetDownlineInfo(Arg.Any<int>(), Arg.Any<string>())
                .Returns(Task.FromResult(downlineInfo));

            var expectedResult = Result.CreateSuccessfulResult();

            // Act
            var actualResult = validator.ValidateAction(1, user, "abc");

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }
    }
}