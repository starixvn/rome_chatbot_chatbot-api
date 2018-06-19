namespace Messenger.WebHooks
{
    using System.Web.Http;
    using Fanex.Logging.Extensions.Web;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ExceptionHandler.UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());
        }
    }
}