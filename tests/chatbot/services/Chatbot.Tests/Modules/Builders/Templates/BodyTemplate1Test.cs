namespace Chatbot.Tests.Modules.Builders.Templates
{
    using System.Globalization;
    using Chatbot.Modules.MessageBuilders.Templates;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "Buildes/Templates")]
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
    }
}