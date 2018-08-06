namespace Chatbot.WebApi.Params
{
    public class UserParam
    {
        public int CustId { get; set; }

        public int SubAccountId { get; set; }

        public string Language { get; set; }

        public int MessengerType { get; set; }

        public string CustName { get; set; }

        public string MessengerUsername { get; set; }
    }
}