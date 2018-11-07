namespace Chatbot.Modules.Actions.Models
{
    public class Action
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Command { get; private set; }

        public int DisplayOrder { get; private set; }

        public int FunctionId { get; private set; }

        public int ParentId { get; private set; }

        public bool IsEndAction { get; private set; }

        public int Status { get; private set; }

        public byte Type { get; private set; }

        public Action NextAction { get; private set; }
    }
}