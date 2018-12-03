namespace Chatbot.Modules.Actions.Models
{
    using System;
    using System.Collections.Generic;

    public class ButtonData
    {
        public int ActionId { get; set; }

        public IList<UserAction> UserActions { get; set; }

        public bool IsCancel { get; set; }

        public string CompiledCommand { get; set; }

        public DateTime SentTime { get; set; }

        public bool IsConfirmButton { get; set; }

        public string Label { get; set; }
    }
}