namespace Chatbot.Tests.Modules.Builders.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Chatbot.Modules.MessageBuilders.Templates;
    using Xunit;

    public class HeaderTemplateTests
    {
        [Fact]
        public void ToString_Always_ReturnHeaderFunctionString()
        {
            var headerTemplate = new HeaderTemplate();
            var cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

            var headerFunctionString = headerTemplate.ToString(1, cultureInfo);

            Assert.Equal("Outstanding", headerFunctionString);
        }
    }
}
