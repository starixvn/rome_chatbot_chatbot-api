using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.Chatbot.Client.Services;

namespace Alpha.Chatbot.Client.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var settingService = new SettingServiceImpl();

            var functions = settingService.GetFunctions(1);


        }
    }
}
