namespace Chatbot.Services
{
    using System.Threading.Tasks;

    public interface IMessageBuilder
    {
        string BuildVerificationCodeMessage(string verificationCode);
    }

    public class MessageBuilder : IMessageBuilder
    {
        public string BuildVerificationCodeMessage(string verificationCode)
            => $"Your Letstalk validation code is {verificationCode}. The validation code will be expired after 5 minutes.";
    }
}