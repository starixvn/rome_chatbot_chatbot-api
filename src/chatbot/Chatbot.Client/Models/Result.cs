namespace Chatbot.Client.Models
{
    public class Result
    {
        public const int VerificationCodeExpiredCode = 3;
        public const int WrongVerificationCode = 4;
        public const int OverMaxAddAccountTimes = 6;

        public Result() { }

        public Result(int code, string message = "")
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }

        public bool IsVerificationCodeExpired
            => Code == VerificationCodeExpiredCode;

        public static Result CreateVerificationCodeExpiredResult()
            => new Result(VerificationCodeExpiredCode);

        public bool IsWrongVerificationCode
            => Code == WrongVerificationCode;

        public static Result CreateWrongVerificationCodeResult()
            => new Result(WrongVerificationCode);

        public bool IsOverMaxAddAccountTimes
            => Code == OverMaxAddAccountTimes;

        public static Result CreateIsOverMaxAddAccountTimesResult()
            => new Result(OverMaxAddAccountTimes);

        public static Result CreateSuccessfulResult()
            => new Result(0);
    }

    public class AddingUserResult : Result
    {
        private const int MessengerAccountDoesNotExist = 2;
        public const int MessengerAccountHasBeenRegisteredByAnotherCustomer = 5;

        public AddingUserResult() { }

        public AddingUserResult(int code, string message = "", int userId = 0) : base(code, message)
        {
            UserId = userId;
        }

        public static AddingUserResult CreateSuccessfulResult(int userId)
            => new AddingUserResult(0, string.Empty, userId);

        public static AddingUserResult CreateAccountDoesNotExistResult()
            => new AddingUserResult(MessengerAccountDoesNotExist);

        public static AddingUserResult CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult()
            => new AddingUserResult(MessengerAccountHasBeenRegisteredByAnotherCustomer);

        public bool IsMessengerAccountHasBeenRegisteredByAnotherCustomer
            => Code == MessengerAccountHasBeenRegisteredByAnotherCustomer;

        public int UserId { get; set; }
    }
}