namespace Chatbot.Client.Sample
{
    using System;
    using Chatbot.Client.Services;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ChatbotClientManager
                .UseConfig(AppSettings.BotClientConfigs)
                .UseRestClient(new Transport.RestClient());

            var settingService = new SettingServiceImpl();

            //var functions = settingService.GetFunctions(1).Result;

            var addUserResult = settingService.AddUser(1, 2, "en-us", "demo");

            //settingService.UpdateLanguage(1, "fdfd");

            //settingService.UpdateFunctions(
            //    1,
            //    new List<Function>
            //    {
            //        new Function
            //        {
            //            Id = 1,
            //            Name = "fdfdfd"
            //        }
            //    });

            //settingService.GenerateVerificationCode(1, "ffff");

            //settingService.VerifyVerificationCode(1, "23232", "ffff");

            //settingService.AddUser(1, 2, "4", "5");

            //settingService.GetUser(3, 4);

            //settingService.GetAllFunctions();

            //settingService.Unsubscribe(3, 4);
            Console.ReadLine();
        }
    }
}