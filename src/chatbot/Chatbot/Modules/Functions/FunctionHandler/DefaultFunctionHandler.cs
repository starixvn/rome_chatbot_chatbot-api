namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Fanex.Logging;
    using Result = Models.Result;

    public interface IFunctionHandler
    {
        Task<Result> Handle(FunctionParam param);

        Task<IEnumerable<FunctionMessage>> GetFunctionMessages(FunctionParam param);
    }

    public class DefaultFunctionHandler : IFunctionHandler
    {
        protected readonly IMessageBuilder messageBuilder;
        protected readonly IMessageSender messageSender;
        protected readonly CustomerService customerService;
        private readonly IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers;
        private readonly ILogger logger;

        protected bool SendMessageInCaseAccountClosedOrLocked = true;

        public DefaultFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            ILogger logger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null)
        {
            this.messageBuilder = messageBuilder;
            this.messageSender = messageSender;
            this.getDataHandlers = getDataHandlers;
            this.customerService = customerService;
            this.logger = logger;
        }

        public virtual async Task<Result> Handle(FunctionParam param)
        {
            // Check Params
            if (!getDataHandlers.ContainsKey(param.Function.Id))
            {
                throw new InvalidOperationException($"Cannot find get data function for function: {param.Function.Id}-{param.Function.Name}");
            }

            var accountStatus = await GetAccountStatus(param.User).ConfigureAwait(false);

            if (!accountStatus.IsActive || accountStatus.IsLocked)
            {
                return await BuildAndSendAccountClosedOrLockedMessage(param, !accountStatus.IsActive).ConfigureAwait(false);
            }

            return await RunHandler(param).ConfigureAwait(false);
        }

        protected internal virtual async Task<Result> RunHandler(FunctionParam param)
        {
            var data = await getDataHandlers[param.Function.Id](param).ConfigureAwait(false);

            if (data == null)
            {
                return Result.CreateSuccessfulResult("No data");
            }

            return await BuildAndSendMessage(param, data).ConfigureAwait(false);
        }

        protected Task<AccountStatus> GetAccountStatus(User user)
            => customerService.GetAccountStatus(user.CustId, user.SubAccountId);

        protected virtual Task<Result> BuildAndSendAccountClosedOrLockedMessage(FunctionParam param, bool isClosed)
        {
            if (SendMessageInCaseAccountClosedOrLocked)
            {
                var message = isClosed
                    ? messageBuilder.BuildAccountClosedMessage(param.User.Language)
                    : messageBuilder.BuildAccountLockedMessage(param.User.Language);

                return messageSender.SendMessage(
                    param.User.Id,
                    param.Messenger,
                    message,
                    param.User.SelfdestructionPeriod,
                    param.Function.Id);
            }

            return Task.FromResult(Result.CreateSuccessfulResult());
        }

        protected virtual async Task<Result> BuildAndSendMessage(FunctionParam param, object data)
        {
            string message = BuildMessage(param.User.Language, param.Function.Id, param.Messenger.Type, data);

            return await messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id).ConfigureAwait(false);
        }

        protected string BuildMessage(string language, int functionId, int messengerType, object data)
            => messageBuilder.BuildCommandResultMessage(
                    language,
                    functionId,
                    messengerType,
                    data);

        public virtual Task<IEnumerable<FunctionMessage>> GetFunctionMessages(FunctionParam param)
            => Task.FromResult<IEnumerable<FunctionMessage>>(new List<FunctionMessage>());

        protected void LogError(User user, Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Function Handler Exception",
                                    $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                                    ex.ToString());

            logger.Error(logMessage);
        }

        protected void LogError(Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Function Handler Exception",
                                    ex.ToString());

            logger.Error(logMessage);
        }

        protected static bool IsValidDirectDownline(int custLevel, DownlineInfo downlineInfo)
            => downlineInfo != null
                && downlineInfo.CustId > 0
                && IsDirectDownline(custLevel, downlineInfo.RoleId);

        private static bool IsDirectDownline(int custLevel, int downlineLevel)
            => (custLevel - downlineLevel) == 1;

        protected Task<Result> BuildAndSendInvalidNumerOfDownlineRangeMessage(FunctionParam param, int maxTopQuery)
        {
            var message = messageBuilder.BuildInvalidNumerOfDownlineRange(param.User.Language, maxTopQuery);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }
    }
}