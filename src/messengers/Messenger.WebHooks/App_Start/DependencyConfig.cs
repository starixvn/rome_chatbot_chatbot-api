namespace Messenger.WebHooks.App_Start
{
    using Fanex.Bot.Client;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using SimpleInjector;

    public static class DependencyConfig
    {
        public static void Init()
        {
            var container = new Container();

            container.RegisterSingleton<IBotConnector, BotConnector>();
            container.RegisterSingleton<IRestClientHelper, RestClientHelper>();
            container.RegisterSingleton<IMessengerClient, LetstalkClient>();
            container.RegisterSingleton<IJwtHelper, JwtHelper>();
            container.RegisterSingleton<ICryptographyHelper, CryptographyHelper>();
            container.RegisterInstance(AppSettings.Messenger);

            container.Register(() => RestClientHelper.CreateDefaultClient(AppSettings.Messenger.ApiEndpoint), Lifestyle.Singleton);

            container.Verify();

            DependencyResolver.SetupContainer(container);
        }
    }
}