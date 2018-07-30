namespace Chatbot.Models.Dtos
{
    using System;

    public class MessengerDto
    {
        public int UserId { get; set; }

        public int Type { get; set; }

        public string Username { get; set; }

        public bool Enabled { get; set; }

        public bool IsVerifying { get; set; }

        public string VerificationCode { get; set; }

        public DateTime GeneratedVerificationCodeTime { get; set; }
    }
}