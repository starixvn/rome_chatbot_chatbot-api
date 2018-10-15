namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System;
    using System.Globalization;

    public class HeaderTemplate4 : HeaderTemplate
    {
        public override string ToString(int functionId, CultureInfo cultureInfo)
         => $"[{GetResource($"Header_{functionId}", cultureInfo)} - {DateTime.Now.AddDays(-1).ToString("yyyyMMdd")}]";
    }
}