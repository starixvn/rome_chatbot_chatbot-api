namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    public class GetMessengerByUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetMessengerByUserCriteria(0, 0, string.Empty);

            // Assert
            Assert.Equal("GetMessengerByUser", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnFalse(int userId)
        {
            // Act
            var criteria = new GetMessengerByUserCriteria(userId);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidUserId_ReturnsTrue()
        {
            // Act
            var criteria = new GetMessengerByUserCriteria(1);

            // Assert
            Assert.True(criteria.IsValid());
        }
    }
}