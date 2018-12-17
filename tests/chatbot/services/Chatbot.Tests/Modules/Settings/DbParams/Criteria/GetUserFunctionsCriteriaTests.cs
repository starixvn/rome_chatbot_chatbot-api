namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    public class GetUserFunctionsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetUserFunctionsCriteria(0);

            // Assert
            Assert.Equal("GetUserFunctions", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidSettingName_ReturnsFalse(int userId)
        {
            // Act
            var criteria = new GetUserFunctionsCriteria(userId);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidSettingName_ReturnsFalse()
        {
            // Act
            var criteria = new GetUserFunctionsCriteria(1);

            // Assert
            Assert.True(criteria.IsValid());
        }
    }
}