namespace Alpha.Chatbot.Client.Models
{
    public class Result
    {
        public int Code { get; set; }

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }
    }
}