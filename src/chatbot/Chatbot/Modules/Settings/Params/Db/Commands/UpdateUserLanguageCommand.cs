namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public class UpdateUserLanguageCommand : NonQueryCommand
    {
        public UpdateUserLanguageCommand(int userId, string language)
        {
            UserId = userId;
            Language = language;
        }

        public int UserId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "UpdateUserLanguage";

        public override bool IsValid()
            => UserId > 0
            && !string.IsNullOrWhiteSpace(Language);
    }
}