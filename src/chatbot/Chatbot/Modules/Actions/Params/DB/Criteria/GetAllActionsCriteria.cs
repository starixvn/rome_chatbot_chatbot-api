namespace Chatbot.Modules.Actions.Params.DB.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllActionsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "GetAllActions";

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}