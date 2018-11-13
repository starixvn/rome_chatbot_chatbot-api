namespace Chatbot.Modules.SettingLogs.Constants
{
    using System.Collections.Generic;

    public static class SelfdestructionPeriod
    {
        public const int FifteenMinutes = 15;
        public const int ThirtyMinutes = 30;
        public const int OneHour = 60;
        public const int ThreeHours = 180;
        public const int SixHours = 360;
        public const int TwelveHours = 720;
        public const int OneDay = 1440;
        public const int ThreeDays = 4320;
        public const int SevenDays = 10080;
        public const int FifteenDays = 21600;
        public const int ThirtyDays = 43200;

        public static IDictionary<int, string> Options { get; } = new Dictionary<int, string>
        {
            [FifteenMinutes] = "15 minutes",
            [ThirtyMinutes] = "30 minutes",
            [OneHour] = "1 hour",
            [ThreeHours] = "3 hours",
            [SixHours] = "6 hours",
            [TwelveHours] = "12 hours",
            [OneDay] = "1 day",
            [ThreeDays] = "3 days",
            [SevenDays] = "7 days",
            [FifteenDays] = "15 days",
            [ThirtyDays] = "30 days"
        };
    }
}