namespace Chatbot.WebApi.Params
{
    public class VerificationCodeParam
    {
        public string VerificationCode { get; set; }

        public int MessengerType { get; set; }

        public string MessengerUsername { get; set; }
    }
}