namespace Chatbot.Schedule.Service.MessageHandler.Builders
{
    using System.Collections.Generic;

    public interface IMessageBuilder
    {
        string GetMessage(IDictionary<string, object> parameters);
    }
}