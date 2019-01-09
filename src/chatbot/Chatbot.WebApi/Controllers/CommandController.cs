namespace Chatbot.WebApi.Controllers
{
    using System.Threading.Tasks;
    using Chatbot.Modules.Funtions;
    using Chatbot.WebApi.Filters;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/command")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly CommandService commandService;

        public CommandController(CommandService commandService)
        {
            this.commandService = commandService;
        }

        [ServiceFilter(typeof(AutoReplyUMFilterAttribute)), ServiceFilter(typeof(ForceUMFilterAttribute))]
        [Route("run"), HttpPost]
        public async Task Run([FromBody]CommandParam param)
        {
            await commandService.Execute(
                param.Command,
                param.ButtonData,
                param.MessengerType,
                param.MessengerUsername);
        }
    }
}