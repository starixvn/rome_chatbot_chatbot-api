namespace Chatbot.Models.Dtos
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(
            int userId,
            int custId,
            int subAccountId,
            string language)
        {
            UserId = userId;
            CustId = custId;
            SubAccountId = subAccountId;
            Language = language;
        }

        public int UserId { get; private set; }

        public int CustId { get; private set; }

        public int SubAccountId { get; private set; }

        public string Language { get; private set; }
    }
}