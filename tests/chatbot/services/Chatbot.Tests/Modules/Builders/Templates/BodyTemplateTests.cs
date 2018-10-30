namespace Chatbot.Tests.Modules.Builders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Chatbot.Modules.MessageBuilders.Templates;
    using NSubstitute;
    using Xunit;

    public class BodyTemplateTests
    {
        private readonly BodyTemplate bodyTemplate;
        private readonly CultureInfo cultureInfo;
        private readonly IMessengerFormatter messengerFormatter;

        public BodyTemplateTests()
        {
            bodyTemplate = new BodyTemplate();
            messengerFormatter = Substitute.For<IMessengerFormatter>();
            cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
        }

        [Fact]
        public void ToString_NoData_ReturnNoDataText()
        {
            var data = new Dictionary<string, object>();

            var template = bodyTemplate.ToString(data, cultureInfo, messengerFormatter);

            Assert.Equal("No information is available", template);
        }

        [Fact]
        public void ToString_Always_ReturnFormattedTemplate()
        {
            var data = new Dictionary<string, object>
            {
                { "Super", 3434 },
                { "Master", 4545.555 },
                { "param3", "value3" },
                { "param4", true }
            };
            messengerFormatter.NewLine().Returns("_newline_");
            messengerFormatter.Tab().Returns("_tab_");

            var template = bodyTemplate.ToString(data, cultureInfo, messengerFormatter);

            Assert.Equal("Super:_newline__tab_3434_newline_Master:_newline__tab_4,545.56_newline_:_newline__tab_value3_newline_:_newline__tab_True", template);
        }
    }
}