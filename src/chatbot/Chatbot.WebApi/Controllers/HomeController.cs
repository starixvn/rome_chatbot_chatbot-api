namespace Chatbot.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var message = "Chatbot API has been started.";
            var contentResult = new ContentResult();
            contentResult.Content = message;
            _logger.LogInformation(message);
            return contentResult;
        }
    }
}