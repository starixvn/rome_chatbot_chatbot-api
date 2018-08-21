namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using Fanex.Data.Repository;

    public class GetCustomerStatusCommand : NonQueryCommand
    {
        public GetCustomerStatusCommand(int custId, int subAccountId)
        {
            SubAccId = subAccountId;
            CustId = custId;
        }

        public bool IsActive { get; set; }

        public int CustId { get; }

        public int SubAccId { get; }

        public override string GetSettingKey() => "Age_CustomerStatusSel";

        public override bool IsValid()
            => CustId > 0;
    }
}