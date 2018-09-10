namespace Chatbot.Tests.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Chatbot.Configurations;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Xunit;

    public class ChatbotConfigsTests
    {
        [Fact]
        public void Ctor_Always_ReadConfigs()
        {
            var configuration = Substitute.For<IConfiguration>();

            var config = new ChatbotConfigs(configuration);
        }
    }
}
