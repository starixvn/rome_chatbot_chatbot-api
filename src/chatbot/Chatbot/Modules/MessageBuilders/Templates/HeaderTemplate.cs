namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System;
    using System.Globalization;
    using Chatbot.AppResources;

    public interface IHeaderTemplate
    {
        string ToString(int functionId, CultureInfo cultureInfo);
    }

    public class HeaderTemplate : IHeaderTemplate
    {
        protected virtual string GetResource(string key, CultureInfo cultureInfo)
            => Languages.ResourceManager.GetString(key, cultureInfo);

        public virtual string ToString(int functionId, CultureInfo cultureInfo)
            => $"[{GetResource($"Header_{functionId}", cultureInfo)} - {DateTime.Now.ToString("yyyy/MM/dd")}]";
    }
}