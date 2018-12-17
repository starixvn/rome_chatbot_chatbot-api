namespace Chatbot.Tests.Modules.Settings.DbParams.Criteria
{
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Xunit;

    public class GetSettingCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetSettingCriteria(string.Empty);

            // Assert
            Assert.Equal("GetSetting", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsValid_InvalidSettingName_ReturnsFalse(string settingName)
        {
            // Act
            var criteria = new GetSettingCriteria(settingName);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidSettingName_ReturnsTrue()
        {
            // Act
            var criteria = new GetSettingCriteria("setting name");

            // Assert
            Assert.True(criteria.IsValid());
        }
    }
}