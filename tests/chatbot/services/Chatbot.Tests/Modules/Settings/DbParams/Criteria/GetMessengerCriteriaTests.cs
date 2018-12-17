namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    public class GetMessengerCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetMessengerCriteria(string.Empty, 0, false);

            // Assert
            Assert.Equal("GetMessenger", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_InvalidMessengerUserName_ReturnsFalse(string messengerUsername)
        {
            // Act
            var criteria = new GetMessengerCriteria(messengerUsername, 1, false);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidMessengerType_ReturnsFalse(int messengerType)
        {
            // Act
            var criteria = new GetMessengerCriteria("username", messengerType, false);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCriteria_ReturnsTrue()
        {
            // Act
            var criteria = new GetMessengerCriteria("username", 1, false);

            // Assert
            Assert.True(criteria.IsValid());
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Status_Always_ReturnsCorrectValue(bool enable, int expectedResult)
        {
            // Act
            var criteria = new GetMessengerCriteria("username", 1, enable);

            // Assert
            Assert.Equal(expectedResult, criteria.Status);
        }
    }
}