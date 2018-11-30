namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;

    public class DirectDownlineActionValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CustomerService customerService;

        public DirectDownlineActionValidator(
            IMessageBuilder messageBuilder,
            CustomerService customerService)
        {
            this.messageBuilder = messageBuilder;
            this.customerService = customerService;
        }

        public Result ValidateAction(User user, string userResponse, IList<UserAction> userActions)
        {
            if (!string.IsNullOrWhiteSpace(userResponse))
            {
                var downlineInfo = customerService.GetDownlineInfo(user.CustId, userResponse)
                    .GetAwaiter().GetResult();

                if (downlineInfo != null
                    && downlineInfo.CustId > 0
                    && IsDirectDownline(user.CustLevel, downlineInfo.RoleId))
                {
                    return Result.CreateSuccessfulResult();
                }
            }

            return Result.CreateErrorResult(
                messageBuilder.BuildInvalidDirectDownlineMessage(
                    userResponse,
                    user.CustLevel - 1,
                    user.Language));
        }

        private static bool IsDirectDownline(int custLevel, int downlineLevel)
            => (custLevel - downlineLevel) == 1;
    }
}