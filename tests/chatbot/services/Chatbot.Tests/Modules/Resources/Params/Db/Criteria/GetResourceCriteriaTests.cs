using Chatbot.Modules.Resources.Constants;
using Chatbot.Modules.Resources.Params.Db.Criteria;
using Xunit;

namespace Chatbot.Tests.Modules.Resources.Params.Db.Criteria
{
    public class GetResourceCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectKey()
        {
            // Arrange
            var criteria = new GetResourceCriteria(0, string.Empty);

            // Act & Assert
            Assert.Equal("Chatbot_GetResource", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidKey_ReturnsFalse(int key)
        {
            // Arrange
            var criteria = new GetResourceCriteria(key, ResourceType.Function);

            // Act & Assert
            Assert.False(criteria.IsValid());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsValid_InvalidType_ReturnsFalse(string type)
        {
            // Arrange
            var criteria = new GetResourceCriteria(1, type);

            // Act & Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCriteria_ReturnsTrue()
        {
            // Arrange
            var criteria = new GetResourceCriteria(1, ResourceType.Function);

            // Act & Assert
            Assert.True(criteria.IsValid());
        }
    }
}