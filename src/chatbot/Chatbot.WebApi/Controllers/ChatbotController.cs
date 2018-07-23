namespace Chatbot.WebApi.Controllers
{
    using System.Collections.Generic;
    using Chatbot.Models;
    using Microsoft.AspNetCore.Mvc;

    public class ChatbotController : Controller
    {
        public ChatbotController()
        {
                
        }

        [Route("api/bot/{custId}/setting"), HttpGet]
        public IEnumerable<Function> Setting(int custId)
        {
            return new List<Function>
            {
                new Function { Id = 1, Name = "Outstanding", Enabled = true },
                new Function { Id = 2, Name = "WinLoss", Enabled = false },
            };
        }

        [Route("api/bot/{custId}/setting"), HttpPost]
        public IEnumerable<Function> Setting(int custId, [FromBody]IEnumerable<Function> settingItems)
        {
            return settingItems;
        }
    }
}