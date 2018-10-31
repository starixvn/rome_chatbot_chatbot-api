namespace Chatbot.Tests.Modules.Builders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Chatbot.Modules.MessageBuilders.Templates;
    using NSubstitute;
    using Xunit;

    public class BodyTemplate1Test
    {
        private readonly BodyTemplate1 bodyTemplate;
        private readonly CultureInfo cultureInfo;
        private readonly IMessengerFormatter messengerFormatter;

        public BodyTemplate1Test()
        {
            bodyTemplate = new BodyTemplate1();
            cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
            messengerFormatter = Substitute.For<IMessengerFormatter>();
        }

        [Fact]
        public void ToString_DataIsNotOutstandingDetail_ReturnFormattedTemplate()
        {
            // Arrange
            var data = new Dictionary<string, object>
            {
                { "Username", "username" },
                { "Member", 4545.555 },
                { "Agent", 123.456 }
            };

            messengerFormatter.NewLine().Returns("\n");
            messengerFormatter.Tab().Returns("\t\t");

            // Act
            var template = bodyTemplate.ToString(data, cultureInfo, messengerFormatter);

            // Assert
            Assert.Equal("Username:\n\t\tusername\nMember:\n\t\t4,545.56\nAgent:\n\t\t123.46", template);
        }
    }
}