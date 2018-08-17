namespace Chatbot.Models.Dtos
{
    using System;

    public class MessengerDto
    {
        public MessengerDto() { }

        internal MessengerDto(
            int userId,
            int type,
            string username,
            bool enabled,
            bool isVerifying,
            string verificationCode,
            DateTime generatedVerificationCodeTime)
        {
            UserId = userId;
            Type = type;
            Username = username;
            Enabled = enabled;
            IsVerifying = isVerifying;
            VerificationCode = verificationCode;
            GeneratedVerificationCodeTime = generatedVerificationCodeTime;
        }

        public int UserId { get; private set; }

        public int Type { get; private set; }

        public string Username { get; private set; }

        public bool Enabled { get; private set; }

        public bool IsVerifying { get; private set; }

        public string VerificationCode { get; private set; }

        public DateTime GeneratedVerificationCodeTime { get; private set; }
    }
}