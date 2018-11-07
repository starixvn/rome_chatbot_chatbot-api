namespace Chatbot.Modules.Resources.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetResourceCriteria : CriteriaBase
    {
        public GetResourceCriteria(int key, string type)
        {
            Key = key;
            Type = type;
        }

        public int Key { get; }

        public string Type { get; }

        public override string GetSettingKey() => "Chatbot_GetResource";

        public override bool IsValid() => Key > 0 && !string.IsNullOrWhiteSpace(Type);
    }
}