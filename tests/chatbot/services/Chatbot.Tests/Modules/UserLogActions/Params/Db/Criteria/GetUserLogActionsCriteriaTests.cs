namespace Chatbot.Tests.Modules.UserLogActions.Params.Db.Criteria
{
    using Chatbot.Modules.UserLogActions.Params.Db.Criteria;
    using Xunit;

    public class GetUserLogActionsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectSettingKey()
        {
            // Act
            var criteria = new GetUserLogActionsCriteria(0, 0, 0, string.Empty);

            // Assert
            Assert.Equal("GetUserLogActions", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnsFalse(int userId)
        {
            // Arrange
            var criteria = new GetUserLogActionsCriteria(userId, 1, 1, string.Empty);

            // Act & Assert
            Assert.False(criteria.IsValid());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidActionId_ReturnsFalse(int actionId)
        {
            // Arrange
            var criteria = new GetUserLogActionsCriteria(1, actionId, 1, string.Empty);

            // Act & Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCriteria_ReturnsTrue()
        {
            // Arrange
            var criteria = new GetUserLogActionsCriteria(1, 1, 1, string.Empty);

            // Act & Assert
            Assert.True(criteria.IsValid());
        }
    }
}