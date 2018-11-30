namespace Chatbot.Modules.Functions.FunctionHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Fanex.Logging;

    public class CopyCustomerFunctionHandler : DefaultFunctionHandler
    {
        private readonly CopyCustomerService copyCustomerService;

        public CopyCustomerFunctionHandler(
            IMessageBuilder messageBuilder,
            IMessageSender messageSender,
            CustomerService customerService,
            CopyCustomerService copyCustomerService,
            ILogger logger,
            IDictionary<int, Func<FunctionParam, Task<object>>> getDataHandlers = null)
            : base(messageBuilder, messageSender, customerService, logger, getDataHandlers)
        {
            this.copyCustomerService = copyCustomerService;
        }

        protected override async Task<Result> RunHandler(FunctionParam param)
        {
            var rawParameters = param.Others["parameters"] as string[];

            var validateResult = await ValidateRawParamerters(rawParameters, param).ConfigureAwait(false);
            if (validateResult.IsNotOk)
            {
                return validateResult;
            }

            var sourceUsername = rawParameters[0];
            var downlineInfo = await customerService.GetDownlineInfo(param.User.CustId, sourceUsername).ConfigureAwait(false);
            if (!IsValidDirectDownline(param.User.CustLevel, downlineInfo))
            {
                return await BuildAndSendInvalidDirectDownlineMessage(sourceUsername, param).ConfigureAwait(false);
            }

            var pt = await copyCustomerService.GetPT(param.User.CustId, (byte)param.User.CustLevel).ConfigureAwait(false);
            var credit = decimal.Parse(rawParameters[1]);
            if (credit > pt.MaxCredit)
            {
                return await BuildAndSendInvalidMaxCreditMessage(pt.MaxCredit, param.User.CustLevel - 1, param).ConfigureAwait(false);
            }

            if (param.User.CustLevel == CustLevel.Master.Value
                && decimal.Parse(rawParameters[2]) > credit)
            {
                return await BuildAndSendInvalidMaxCreditMessage(credit, CustLevel.Member.Value, param).ConfigureAwait(false);
            }

            var customer = await customerService.GetCustomer(param.User.CustId).ConfigureAwait(false);
            param.Others.Add(nameof(customer.UserName), $"{customer.UserName}{pt.Suffix}");
            param.Others.Add("SourceCustId", downlineInfo.CustId);

            await BuildAndSendCopyCustomerInformMessage(param).ConfigureAwait(false);

            return await base.RunHandler(param).ConfigureAwait(false);
        }

        private async Task<Result> ValidateRawParamerters(string[] rawParameters, FunctionParam param)
        {
            var numberOfRequiredParams = param.User.CustLevel == CustLevel.Master.Value ? 3 : 2;

            if (rawParameters.Length <= 0 || rawParameters.Length != numberOfRequiredParams)
            {
                return await BuildAndSendNotRecognizedCommandMessage(param).ConfigureAwait(false);
            }

            var sourceUsername = rawParameters[0];
            if (string.IsNullOrWhiteSpace(sourceUsername))
            {
                return await BuildAndSendInvalidDirectDownlineMessage(sourceUsername, param).ConfigureAwait(false);
            }

            if (!Decimal.TryParse(rawParameters[1], out decimal credit))
            {
                return await BuildAndSendBuildInvalidNumericMessage(rawParameters[1], param).ConfigureAwait(false);
            }

            if (param.User.CustLevel == CustLevel.Master.Value
                && !Decimal.TryParse(rawParameters[2], out decimal givenMemberCredit))
            {
                return await BuildAndSendBuildInvalidNumericMessage(rawParameters[2], param).ConfigureAwait(false);
            }

            return Result.CreateSuccessfulResult();
        }

        private Task<Result> BuildAndSendNotRecognizedCommandMessage(FunctionParam param)
        {
            var message = messageBuilder.BuildNotRecognizedCommandMessage(param.User.Language);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
        }

        private Task<Result> BuildAndSendBuildInvalidNumericMessage(string value, FunctionParam param)
        {
            var message = messageBuilder.BuildInvalidNumericMessage(value, param.User.Language);

            return messageSender.SendMessage(
                param.User.Id,
                param.Messenger,
                message,
                param.User.SelfdestructionPeriod,
                param.Function.Id);
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
    }
}