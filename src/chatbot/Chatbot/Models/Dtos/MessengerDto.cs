namespace Chatbot.Models.Dtos
{
    using System;

    public class MessengerDto
    {
        public int UserId { get; set; }

        public int Type { get; set; }

        public string Account { get; set; }

        public bool Enabled { get; set; }

        public string VerificationCode { get; set; }

        public DateTime GeneratedVerificationCodeTime { get; set; }
    }
}