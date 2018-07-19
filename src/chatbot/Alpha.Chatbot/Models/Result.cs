namespace Alpha.Chatbot.Models
{
    public class Result
    {
        public int Code { get; set; }

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }
    }

    public class AddingMessengerAccountResult : Result
    {
        private const int AccountDoesNotExistCode = 2;

        public static AddingMessengerAccountResult CreateAccountDoesNotExistResult()
            => new AddingMessengerAccountResult { Code = AccountDoesNotExistCode };

        public string VerificationCode { get; set; }
    }
}