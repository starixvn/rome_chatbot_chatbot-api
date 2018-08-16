namespace Chatbot.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DataHelper
    {
        public static T Parse<T>(object data)
                where T : class
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var model = data as T;

            if (model == null)
            {
                throw new InvalidOperationException($"Cannot parse {typeof(T)} model");
            }

            return model;
        }
    }
}