namespace Messenger.WebHooks
{
    using System;
    using System.Web.Http;
    using Fanex.Logging.Extensions.Web;
    using Messenger.WebHooks.App_Start;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            DependencyConfig.Init();

            ExceptionHandler.UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }
    }
}