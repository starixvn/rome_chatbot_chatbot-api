namespace Chatbot.Models
{
    public class Result
    {
        public Result(int code, string message = "")
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; }

        public static Result CreateSuccessfulResult()
            => new Result(0);
    }

    public class VerificationCodeResult : Result
    {
        private const int VerificationCodeExpiredCode = 3;
        private const int WrongVerificationCode = 4;

        public VerificationCodeResult(int code, string message = "", int failedTimes = 0)
            : base(code, message)
        {
            FailedTimes = failedTimes;
        }

        public int FailedTimes { get; }

        public bool IsVerificationCodeExpired
            => Code == VerificationCodeExpiredCode;

        public static VerificationCodeResult CreateVerificationCodeExpiredResult()
            => new VerificationCodeResult(VerificationCodeExpiredCode);

        public bool IsWrongVerificationCode
            => Code == WrongVerificationCode;

        public static VerificationCodeResult CreateWrongVerificationCodeResult(int failedTimes = 0)
            => new VerificationCodeResult(WrongVerificationCode, string.Empty, failedTimes);

        public static VerificationCodeResult CreateVerificationSuccessfulResult()
            => new VerificationCodeResult(0, string.Empty, 0);
    }

    public class AddingUserResult : Result
    {
        private const int OverMaxAddAccountTimes = 6;
        private const int MessengerAccountDoesNotExist = 2;
        private const int MessengerAccountHasBeenRegisteredByAnotherCustomer = 5;

        public AddingUserResult(int code, string message = "", int userId = 0) : base(code, message)
        {
            UserId = userId;
        }

        public static AddingUserResult CreateSuccessfulResult(int userId)
            => new AddingUserResult(0, string.Empty, userId);

        public static AddingUserResult CreateSuccessfulResult()
            => new AddingUserResult(0, string.Empty, 0);

        public static AddingUserResult CreateAccountDoesNotExistResult()
            => new AddingUserResult(MessengerAccountDoesNotExist);

        public static AddingUserResult CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult()
            => new AddingUserResult(MessengerAccountHasBeenRegisteredByAnotherCustomer);

        public bool IsMessengerAccountHasBeenRegisteredByAnotherCustomer
            => Code == MessengerAccountHasBeenRegisteredByAnotherCustomer;

        public bool IsOverMaxAddAccountTimes
           => Code == OverMaxAddAccountTimes;

        public static AddingUserResult CreateIsOverMaxAddAccountTimesResult()
            => new AddingUserResult(OverMaxAddAccountTimes);

        public int UserId { get; }
    }
}