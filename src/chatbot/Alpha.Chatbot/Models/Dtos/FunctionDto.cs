namespace Alpha.Chatbot.Models.Dtos
{
    public class FunctionDto
    {
        public int FunctionId { get; set; }

        public string FunctionName { get; set; }

        public int Type { get; set; }

        public string DefaultCommand { get; set; }
    }
}