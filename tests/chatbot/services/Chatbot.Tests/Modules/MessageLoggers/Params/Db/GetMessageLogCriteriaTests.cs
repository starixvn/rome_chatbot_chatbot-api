namespace Chatbot.Tests.Modules.MessageLoggers.Params.Db
{
    using System;
    using Chatbot.Modules.MessageLoggers.Params.Db;
    using Xunit;

    public class GetMessageLogCriteriaTests
    {
        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectValue()
        {
            // Act
            var criteria = new GetMessageLogCriteria(0, DateTime.Now, DateTime.Now, 0, 0);

            // Assert
            Assert.Equal("Chatbot_GetMessageLog", criteria.GetSettingKey());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValid_InvalidUserId_ReturnsFalse(int userId)
        {
            // Act
            var criteria = new GetMessageLogCriteria(userId, DateTime.Now, DateTime.Now, 0, 0);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_FromDateIsGreaterThanToData_ReturnsFalse()
        {
            // Act
            var criteria = new GetMessageLogCriteria(1, DateTime.Now, DateTime.Now.AddDays(-1), 0, 0);

            // Assert
            Assert.False(criteria.IsValid());
        }

        [Fact]
        public void IsValid_ValidCriteria_ReturnsTrue()
        {
            // Act
            var criteria = new GetMessageLogCriteria(1, DateTime.Now, DateTime.Now, 0, 0);

            // Assert
            Assert.True(criteria.IsValid());
        }
    }
}