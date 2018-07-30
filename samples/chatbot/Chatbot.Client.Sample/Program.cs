namespace Chatbot.Client.Sample
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Chatbot.Client.Services;
    using Chatbot.Client.Models;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ChatbotClientManager
                .UseConfig(AppSettings.BotClientConfigs)
                .UseRestClient(new Transport.RestClient());

            var settingService = new SettingServiceImpl();
            var userId = 402;
            var messengerUsername = "demo1";

            //var functions = settingService.GetFunctions(1).Result;

            //var addUserResult = settingService.AddUser(1000, 3, "en-US", "starix_test1");
            //var user = settingService.GetUser(1000, 3, "en-US");
            //var result = settingService.UpdateLanguage(402, "en-US");
            //var functions = settingService.GetFunctions(4202, "en-US").Result;
            //var firstFunction = functions.ElementAt(0);
            //firstFunction.Command = "bcd";
            //firstFunction.Enabled = false;

            //var updateFunctions = settingService.UpdateFunctions(402, new List<Function>
            //{
            //    firstFunction
            //});

            settingService.GenerateVerificationCode(1000, "starix_test1");
            //settingService.VerifyVerificationCode(1000, "63447418", "starix_test1");
            //settingService.VerifyVerificationCode(userId, "346717", messengerUsername);

            //settingService.AddUser(1, 2, "4", "5");

            //settingService.GetUser(3, 4);

            //settingService.GetAllFunctions();

            //settingService.Unsubscribe(userId, 1, messengerUsername);
            Console.ReadLine();
        }
    }
}