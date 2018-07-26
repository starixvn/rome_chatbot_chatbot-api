namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class UpdateLanguageCommand : NonQueryCommand
    {
        public UpdateLanguageCommand(int userId, string language)
        {
            UserId = userId;
            Language = language;
        }

        public int UserId { get; }

        public string Language { get; }

        public override string GetSettingKey() => "UpdateLanguageCommand";

        public override bool IsValid()
            => UserId > 0
            && !string.IsNullOrWhiteSpace(Language);
    }
}