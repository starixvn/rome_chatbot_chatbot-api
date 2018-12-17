namespace Chatbot.Tests.Modules.Actions.Params.DB.Criteria
{
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Xunit;

    public class GetActionByUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetActionByUserCriteria(0);

            // Assert
            Assert.Equal("Chatbot_GetUserActionsInChain", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnsFalse(int userId)
        {
            // Act
            var criteria = new GetActionByUserCriteria(userId);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCriteria_ReturnsTrue()
        {
            // Act
            var criteria = new GetActionByUserCriteria(1);

            // Assert
            Assert.True(criteria.IsValid());
        }
    }
}