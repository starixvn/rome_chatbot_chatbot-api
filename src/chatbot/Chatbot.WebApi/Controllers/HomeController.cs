namespace Chatbot.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            var contentResult = new ContentResult();
            contentResult.Content = "App has been started.";
            return contentResult;
        }
    }
}