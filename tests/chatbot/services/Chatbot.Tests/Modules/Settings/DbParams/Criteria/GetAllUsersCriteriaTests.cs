namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    public class GetAllUsersCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectKey()
        {
            // Arrange
            var criteria = new GetAllUsersCriteria();

            // Act & Assert
            Assert.Equal("Chatbot_GetAllUsers", criteria.GetSettingKey());
        }

        [Fact]
        public void IsValid_Always_ReturnsTrue()
        {
            // Arrange
            var criteria = new GetAllUsersCriteria();

            // Act & Assert
            Assert.True(criteria.IsValid());
        }
    }
}