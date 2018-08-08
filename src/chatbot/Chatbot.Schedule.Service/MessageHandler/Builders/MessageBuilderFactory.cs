namespace Chatbot.Schedule.Service.MessageHandler.Builders
{
    using System;

    public interface IMessageBuilderFactory
    {
        IMessageBuilder CreateMessageBuilder(int functionId);
    }

    public class MessageBuilderFactory : IMessageBuilderFactory
    {
        public IMessageBuilder CreateMessageBuilder(int functionId)
        {
            var messageBuilderType = Type.GetType($"Chatbot.Schedule.Service.MessageHandler.Builders.MessageBuilder{functionId}");

            return Activator.CreateInstance(messageBuilderType) as IMessageBuilder;
        }
    }
}