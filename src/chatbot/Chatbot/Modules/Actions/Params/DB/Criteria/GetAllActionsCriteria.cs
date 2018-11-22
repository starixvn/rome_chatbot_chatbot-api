namespace Chatbot.Modules.Actions.Params.DB.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllActionsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
            => "Chatbot_GetAllActions";

        public override bool IsValid()
            => true;
    }
}