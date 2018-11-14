namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class GetAllFunctionsCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Alway_ReturnsCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetAllFunctionsCriteria();

            // Act
            var actualResult = criteria.GetSettingKey();

            // Assert
            Assert.Equal("Chatbot_GetAllFunctions", actualResult);
        }

        [Fact]
        public void IsValid_Alway_ReturnsTrue()
        {
            // Arrange
            var criteria = new GetAllFunctionsCriteria();

            // Act
            var actualResult = criteria.IsValid();

            // Assert
            Assert.True(actualResult);
        }
    }
}
