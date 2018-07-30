namespace Chatbot.Params.Db.Commands
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public abstract class BaseCommand : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public int Code { get; set; }

        [SpParam(Ignored = true)]
        public bool IsCommandRunSuccessfully 
            => Code == 0;

        [SpParam(Ignored = true)]
        public bool IsCommandRunFailed => !IsCommandRunSuccessfully;
    }
}