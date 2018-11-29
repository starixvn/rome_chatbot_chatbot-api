namespace Chatbot.WebApi.Params
{
    using Alpha.Services.Client.Models;

    public class CopyCustomerParam : ICopyCustomerData
    {
        public CopyCustomerParam(
            int actorId,
            byte actorUserLevel,
            string username,
            decimal credit,
            decimal maxCredit,
            int sourceCustId,
            string language,
            string ip)
        {
            ActorId = actorId;
            ActorUserLevel = actorUserLevel;
            UserName = username;
            Credit = credit;
            MaxCredit = maxCredit;
            SourceCustId = sourceCustId;
            Language = language;
            IP = ip;
        }

        public int ActorId { get; }

        public byte ActorUserLevel { get; }

        public string UserName { get; }

        public decimal Credit { get; }

        public decimal MaxCredit { get; }

        public int SourceCustId { get; }

        public string Language { get; }

        public string IP { get; }
    }
}