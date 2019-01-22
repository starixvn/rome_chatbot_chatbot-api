namespace Chatbot.Modules.Actions.Validators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;

    public class CopyCustomerReachedLimitValidator : IActionValidator
    {
        private readonly IMessageBuilder messageBuilder;
        private readonly CustomerService customerService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly UserLogActionService userLogActionService;
        private readonly IChatbotConfigs chatbotConfigs;

        public CopyCustomerReachedLimitValidator(
            IMessageBuilder messageBuilder,
            CustomerService customerService,
            UserLogActionService userLogActionService,
            CopyCustomerService copyCustomerService,
            IChatbotConfigs chatbotConfigs)
        {
            this.messageBuilder = messageBuilder;
            this.chatbotConfigs = chatbotConfigs;
            this.customerService = customerService;
            this.copyCustomerService = copyCustomerService;
            this.userLogActionService = userLogActionService;
        }

        public async Task<ActionValidationResult> ValidateAction(User user, string userResponse, IList<UserAction> userActions, Messenger messenger)
        {
            if (user.CustLevel == CustLevel.AgentLevel)
            {
                var validateResult = await copyCustomerService.ValidateMaxNewMember(user.CustId, user.Language);

                if (!validateResult.IsSuccess)
                {
                    return ActionValidationResult.CreateErrorValidationResult(validateResult.Message, true);
                }
            }

            bool isReachedLimitInDay = await userLogActionService.IsUserReachedTheLimitOfActionInDay(
                user.Id,
                messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay);

            if (isReachedLimitInDay)
            {
                return ActionValidationResult.CreateErrorValidationResult(
                    messageBuilder.BuildCopyCustomerReachedLimitMessage(
                        chatbotConfigs.CopyCustomerLimitPerDay,
                        user.Language),
                    true);
            }

            var customer = await customerService.GetCustomer(user.CustId);
            var reachLimitDownlineResult = await copyCustomerService.CheckReachLimitDownline(user.CustId, (byte)user.CustLevel, customer.UserName, user.Language);
            if (!reachLimitDownlineResult.IsSuccess)
            {
                return ActionValidationResult.CreateErrorValidationResult(reachLimitDownlineResult.Message, true);
            }

            return ActionValidationResult.CreateSuccessfulValidationResult();
        }
    }
}