namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class UpdateLanguageCommand : NonQueryCommand
    {
        public int UserId { get; set; }

        public string Language { get; set; }

        public override string GetSettingKey() => "UpdateLanguageCommand";

        public override bool IsValid()
            => UserId > 0
            && !string.IsNullOrWhiteSpace(Language);
    }
}