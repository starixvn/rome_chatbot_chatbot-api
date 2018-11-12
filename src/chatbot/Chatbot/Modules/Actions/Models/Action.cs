namespace Chatbot.Modules.Actions.Models
{
    using Chatbot.Shared.Constants;

    public class Action
    {
        public Action()
        {
        }

#pragma warning disable S107 // Methods should not have too many parameters

        internal Action(
            int id,
            string name,
            string command,
            int displayOrder,
            int functionId,
            int parentId,
            bool isEndAction,
            byte status,
            byte type)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            Id = id;
            Name = name;
            Command = command;
            DisplayOrder = displayOrder;
            FunctionId = functionId;
            ParentId = parentId;
            IsEndAction = isEndAction;
            Status = status;
            Type = type;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Command { get; private set; }

        public int DisplayOrder { get; private set; }

        public int FunctionId { get; private set; }

        public int ParentId { get; private set; }

        public bool IsEndAction { get; private set; }

        public byte Status { get; private set; }

        public bool IsAvailableOnlyOnPro => Status == Environment.PRO;

        public byte Type { get; private set; }
    }
}