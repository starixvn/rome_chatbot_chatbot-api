namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    public class GetUserCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetUserCriteria(0, 0);

            // Assert
            Assert.Equal("GetUser", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidCustId_ReturnsFalse(int custId)
        {
            // Act
            var criteria = new GetUserCriteria(custId, 0);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_InvalidSubAccountId_ReturnFalse()
        {
            // Act
            var criteria = new GetUserCriteria(1, -1);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCustIdAndSubAccountId_ReturnsTrue()
        {
            // Act
            var criteria = new GetUserCriteria(1, 0);

            // Assert
            Assert.True(criteria.IsValid());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnFalse(int userId)
        {
            // Act
            var criteria = new GetUserCriteria(userId);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidUserId_ReturnsTrue()
        {
            // Act
            var criteria = new GetUserCriteria(1);

            // Assert
            Assert.True(criteria.IsValid());
        }
    }
}