namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageLoggers;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Constants;
    using Fanex.Logging;

    public class CopyCustomerFunctionHandler : DefaultFunctionHandler
    {
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly UserLogActionService userLogActionService;
        private readonly CopyCustomerService copyCustomerService;
        private readonly IMessageLogger messageLogger;

        public CopyCustomerFunctionHandler(
            IChatbotConfigs chatbotConfigs,
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            UserLogActionService userLogActionService,
            CustomerService customerService,
            CopyCustomerService copyCustomerService,
            ILogger logger,
            IMessageLogger messageLogger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHandlers)
        {
            this.chatbotConfigs = chatbotConfigs;
            this.copyCustomerService = copyCustomerService;
            this.userLogActionService = userLogActionService;
            this.messageLogger = messageLogger;
        }

        protected internal override async Task<Result> RunHandler(FunctionParam param)
        {
            var rawParameters = param.Others["parameters"] as string[];
            var customer = await customerService.GetCustomer(param.User.CustId).ConfigureAwait(false);

            if (!IsEnoughParamerters(rawParameters, param.User.CustLevel))
            {
                return await BuildAndSendCopyCustomerInstructionMessage(customer.UserName, param).ConfigureAwait(false);
            }

            var validateRawParametersResult = await ValidateRawParamerters(rawParameters, param).ConfigureAwait(false);
            if (validateRawParametersResult.IsNotOk)
            {
                return await messageSender.SendMessage(
                    param.User.Id,
                    param.Messenger,
                    validateRawParametersResult.Message,
                    param.User.SelfdestructionPeriod,
                    param.Function.Id).ConfigureAwait(false);
            }

            var validateReachedLimitResult = await ValidateReachedLitmitOfCopyCustomer(param).ConfigureAwait(false);
            if (validateReachedLimitResult.IsNotOk)
            {
                return await messageSender.SendMessage(
                    param.User.Id,
                    param.Messenger,
                    validateReachedLimitResult.Message,
                    param.User.SelfdestructionPeriod,
                    param.Function.Id).ConfigureAwait(false);
            }

            var sourceUsername = rawParameters[0];
            var downlineInfo = await customerService.GetDownlineInfo(param.User.CustId, sourceUsername).ConfigureAwait(false);
            if (!IsValidDirectDownline(param.User.CustLevel, downlineInfo))
            {
                return await BuildAndSendInvalidDirectDownlineMessage(sourceUsername, param).ConfigureAwait(false);
            }

            var pt = await copyCustomerService.GetPT(param.User.CustId, (byte)param.User.CustLevel).ConfigureAwait(false);
            var credit = decimal.Parse(rawParameters[1]);
            if (credit < 0 || credit > pt.MaxCredit)
            {
                return await BuildAndSendInvalidMaxCreditMessage(pt.MaxCredit, param.User.CustLevel - 1, param).ConfigureAwait(false);
            }

            if (param.User.CustLevel == CustLevel.MasterLevel)
            {
                var memberCredit = decimal.Parse(rawParameters[2]);
                if (memberCredit < 0 || memberCredit > credit)
                {
                    return await BuildAndSendInvalidMaxCreditMessage(credit, CustLevel.MemberLevel, param).ConfigureAwait(false);
                }
            }

            param.Others.Add(nameof(customer.UserName), $"{customer.UserName}{pt.Suffix}");
            param.Others.Add("SourceCustId", downlineInfo.CustId);
            param.Others.Add("SourceUserName", sourceUsername);

            await BuildAndSendCopyCustomerInformMessage(param).ConfigureAwait(false);

            return await base.RunHandler(param).ConfigureAwait(false);
        }

        protected override async Task<Result> BuildAndSendMessage(FunctionParam param, object data)
        {
            string messageToSend = BuildMessage(param.User.Language, param.Function.Id, param.Messenger.Type, data);
            string messageToLog = messageToSend;

            if (messageToSend.Contains("##"))
            {
                var passwordIndex = messageToSend.IndexOf("##", StringComparison.OrdinalIgnoreCase) + 2;
                var passwordLastIndex = messageToSend
                    .Substring(passwordIndex)
                    .IndexOf("#", StringComparison.OrdinalIgnoreCase);
                var password = messageToSend.Substring(passwordIndex, passwordLastIndex);

                messageToLog = messageToSend.Replace(password, "******").Replace("#", string.Empty);
                messageToSend = messageToSend.Replace("#", string.Empty);
            }

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            var result = await messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                messageToSend,
                param.User.SelfdestructionPeriod,
                param.Function.Id,
                false).ConfigureAwait(false);

            if (result.IsOk)
            {
                await messageLogger.LogMessage(CommonId.BotUserId,
                            param.User.Id,
                            param.Messenger,
                            messageToLog,
                            result.IsOk ? 1 : 0,
                            param.Function.Id).ConfigureAwait(false);
            }

            return result;
        }

        private static bool IsEnoughParamerters(string[] rawParameters, int custLevel)
        {
            var numberOfRequiredParams = custLevel == CustLevel.MasterLevel ? 3 : 2;

            if (rawParameters.Length <= 0 || rawParameters.Length != numberOfRequiredParams)
            {
                return false;
            }

            return true;
        }

        private Task<Result> ValidateRawParamerters(string[] rawParameters, FunctionParam param)
        {
            var sourceUsername = rawParameters[0];
            if (string.IsNullOrWhiteSpace(sourceUsername))
            {
                return Task.FromResult(Result.CreateErrorResult(
                    messageBuilder.BuildInvalidDirectDownlineMessage(
                        sourceUsername,
                        param.User.CustLevel - 1,
                        param.User.Language)));
            }

            if (!Decimal.TryParse(rawParameters[1], out decimal credit))
            {
                return Task.FromResult(Result.CreateErrorResult(messageBuilder.BuildInvalidNumericMessage(rawParameters[1], param.User.Language)));
            }

            if (param.User.CustLevel == CustLevel.MasterLevel
                && !Decimal.TryParse(rawParameters[2], out decimal givenMemberCredit))
            {
                return Task.FromResult(Result.CreateErrorResult(messageBuilder.BuildInvalidNumericMessage(rawParameters[2], param.User.Language)));
            }

            return Task.FromResult(Result.CreateSuccessfulResult());
        }

        private async Task<Result> ValidateReachedLitmitOfCopyCustomer(FunctionParam param)
        {
            if (param.User.CustLevel == CustLevel.AgentLevel)
            {
                var validateResult = await copyCustomerService.ValidateMaxNewMember(param.User.CustId, param.User.Language)
                    .ConfigureAwait(false);

                if (!validateResult.IsSuccess)
                {
                    return Result.CreateErrorResult(validateResult.Message);
                }
            }

            var isReachedLimitOfCopyCustomer = await userLogActionService.IsUserReachedTheLimitOfActionInDay(
                param.User.Id,
                param.Messenger.Type,
                chatbotConfigs.CopyCustomerActionId,
                chatbotConfigs.CopyCustomerLimitPerDay).ConfigureAwait(false);

            if (isReachedLimitOfCopyCustomer)
            {
                return Result.CreateErrorResult(
                    messageBuilder.BuildCopyCustomerReachedLimitMessage(
                        chatbotConfigs.CopyCustomerLimitPerDay,
                        param.User.Language));
            }

            return Result.CreateSuccessfulResult();
        }

        private Task<Result> BuildAndSendInvalidDirectDownlineMessage(string username, FunctionParam param)
        {
            var message = messageBuilder.BuildInvalidDirectDownlineMessage(username, param.User.CustLevel - 1, param.User.Language);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        private Task<Result> BuildAndSendInvalidMaxCreditMessage(decimal maxCredit, int custLevel, FunctionParam param)
        {
            var message = messageBuilder.BuildInvalidMaxCreditMessage(custLevel, maxCredit, param.User.Language);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        private async Task BuildAndSendCopyCustomerInformMessage(FunctionParam param)
        {
            var message = messageBuilder.BuildCopyCustomerInformMessage(param.User.Language);

            await messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id).ConfigureAwait(false);
        }

        private Task<Result> BuildAndSendCopyCustomerInstructionMessage(string createrUsername, FunctionParam param)
        {
            var message = messageBuilder.BuildCopyCustomerInstructionMessage(
                createrUsername,
                param.User.CustLevel - 1,
                param.Messenger.Type,
                param.User.Language);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }
    }
}