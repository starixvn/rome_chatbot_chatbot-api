namespace Chatbot.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

#pragma warning disable S1694 // An abstract class should have both abstract and concrete methods
    public abstract class ServiceBase
#pragma warning restore S1694 // An abstract class should have both abstract and concrete methods
    {
        protected static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        protected static void ValidateInputParam<T>(string paramName, IEnumerable<T> paramValue)
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
