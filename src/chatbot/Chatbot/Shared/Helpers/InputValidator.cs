namespace Chatbot.Shared.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class InputValidator
    {
        public static void Validate(string paramName, object paramValue)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void Validate(string paramName, string paramValue)
        {
            if (paramValue == null
                || string.IsNullOrWhiteSpace(paramValue))
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        public static void Validate(string paramName, int paramValue)
        {
            if (paramValue <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        public static void Validate(string paramName, DateTime paramValue)
        {
            if (paramValue == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        public static void Validate<T>(string paramName, IEnumerable<T> paramValue)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!paramValue.Any())
            {
                throw new ArgumentOutOfRangeException($"{paramName} does not contain any item");
            }
        }
    }
}