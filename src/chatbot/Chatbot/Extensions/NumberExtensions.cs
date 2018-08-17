namespace Chatbot.Extensions
{
    using System;

    public static class NumberExtensions
    {
        private const string NumberFormat = "#,##0";
        private const string ZeroString = "0";

        public static string ToFormattedNumber(this double numberValue)
        {
            if (numberValue == 0)
            {
                return ZeroString;
            }

            var result = numberValue.ToString(NumberFormat);

            return numberValue < 0 && !result.StartsWith("-", StringComparison.OrdinalIgnoreCase)
                ? $"-{result}"
                : result;
        }
    }
}