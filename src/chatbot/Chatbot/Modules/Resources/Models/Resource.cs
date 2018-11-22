namespace Chatbot.Modules.Resources.Models
{
    public class Resource
    {
        public Resource()
        {
        }

        public Resource(
            int key,
            string type,
            string languageCode,
            string value)
        {
            Key = key;
            Type = type;
            LanguageCode = languageCode;
            Value = value;
        }

        public int Key { get; }

        public string Type { get; }

        public string LanguageCode { get; }

        public string Value { get; }
    }
}