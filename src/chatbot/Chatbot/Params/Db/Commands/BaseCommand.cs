namespace Chatbot.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public abstract class BaseCommand : NonQueryCommand
    {
        public int? Code { get; set; }

        public bool IsCommandRunSuccessfully 
            => Code.HasValue && Code.Value == 0;

        public bool IsCommandRunFailed => !IsCommandRunSuccessfully;
    }
}