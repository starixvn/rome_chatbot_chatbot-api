namespace Chatbot.Tests.Modules.Builders.Templates
{
    using System;
    using System.Globalization;
    using Chatbot.Modules.MessageBuilders.Templates;
    using Xunit;

    public class HeaderTemplate4Tests
    {
        [Fact]
        public void ToString_Always_ReturnHeaderFunctionString()
        {
            var headerTemplate = new HeaderTemplate4();
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

            var headerFunctionString = headerTemplate.ToString(4, cultureInfo);
            var date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            Assert.Equal($"[Yesterday Win Loss - {date}]", headerFunctionString);
        }
    }
}