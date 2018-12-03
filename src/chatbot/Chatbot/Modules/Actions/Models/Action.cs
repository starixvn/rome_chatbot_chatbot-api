namespace Chatbot.Modules.Actions.Models
{
    using Chatbot.Shared.Constants;

    public class Action
    {
        public Action()
        {
        }

        internal Action(
            int id,
            string name,
            string command,
            int displayOrder,
            int functionId,
            int parentId,
            bool isEndAction,
            byte status,
            byte type,
            int custLevel,
            int backToActionId)
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
            CustLevel = custLevel;
            BackToActionId = backToActionId;
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

        public int CustLevel { get; private set; }
        
        public bool IsNotRootAction() => ParentId > 0;

        public bool IsNotNextActionOf(int actionId) => IsNotRootAction() && ParentId != actionId;

        public int BackToActionId { get; private set; }
    }
}