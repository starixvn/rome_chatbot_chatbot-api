namespace Chatbot.Tests.Modules.Builders.Templates
{
    using System;
    using System.Globalization;
    using Chatbot.Modules.MessageBuilders.Templates;
    using Xunit;

    [Trait("Modules", "Buildes/Templates")]
    public class HeaderTemplateTests
    {
        [Fact]
        public void ToString_Always_ReturnHeaderFunctionString()
        {
            var headerTemplate = new HeaderTemplate();
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

            var headerFunctionString = headerTemplate.ToString(1, cultureInfo);
            var date = DateTime.Now.ToString("yyyyMMdd");

            Assert.Equal($"[Outstanding - {date}]", headerFunctionString);
        }
    }
}