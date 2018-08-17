namespace Chatbot.Models.Dtos
{
    public class FunctionLanguageDto
    {
        public FunctionLanguageDto() { }

        internal FunctionLanguageDto(
            int functionId,
            string functionName,
            string resourceType,
            string languageCode)
        {
            FunctionId = functionId;
            FunctionName = functionName;
            ResourceType = resourceType;
            LanguageCode = languageCode;
        }

        public int FunctionId { get; private set; }

        public string FunctionName { get; private set; }

        public string ResourceType { get; private set; }

        public string LanguageCode { get; private set; }
    }
}