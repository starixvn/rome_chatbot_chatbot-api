using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ChatR.Startup))]
namespace ChatR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}