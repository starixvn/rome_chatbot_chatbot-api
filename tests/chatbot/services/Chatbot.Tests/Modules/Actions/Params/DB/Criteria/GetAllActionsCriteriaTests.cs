namespace Chatbot.Tests.Modules.Actions.Params.DB.Criteria
{
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Xunit;

    public class GetAllActionsCriteriaTests
    {
        private readonly GetAllActionsCriteria criteria;

        public GetAllActionsCriteriaTests()
        {
            criteria = new GetAllActionsCriteria();
        }

        [Fact]
        public void GetSettingKey_Always_ReturnsCorrectKey()
            => Assert.Equal("Chatbot_GetAllActions", criteria.GetSettingKey());

        [Fact]
        public void IsValid_Always_ReturnsTrue()
            => Assert.True(criteria.IsValid());
    }
}