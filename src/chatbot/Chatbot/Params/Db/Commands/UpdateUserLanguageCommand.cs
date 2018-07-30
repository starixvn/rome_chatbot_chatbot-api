namespace Chatbot.Params.Db.Commands
{
    public class UpdateUserLanguageCommand : BaseCommand
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