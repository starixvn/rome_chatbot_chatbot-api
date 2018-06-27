namespace Messenger.WebHooks.App_Start
{
    using Fanex.Bot.Client;
    using Messenger.Client;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using Messenger.Client.Skype;
    using RestSharp;
    using RestSharp.Authenticators;
    using SimpleInjector;

    public static class DependencyConfig
    {
        public static void Init()
        {
            var container = new Container();

            container.RegisterSingleton<IBotConnector, BotConnector>();
            container.RegisterSingleton<IRestClientHelper, RestClientHelper>();
            container.RegisterSingleton<IMessengerClient, SkypeClient>();

            container.Register<IRestClient>(() => new RestClient
            {
                BaseUrl = new System.Uri(AppSettings.MessengerApiEndpoint),
                Authenticator = new JwtAuthenticator(AppSettings.MessengerApiKey)
            }, Lifestyle.Singleton);

            container.Verify();

            DependencyResolver.SetupContainer(container);
        }
    }
}