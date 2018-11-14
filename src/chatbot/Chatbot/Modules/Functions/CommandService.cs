namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;
    using Chatbot.Shared.Constants;
    using Fanex.Logging;
    using Models;
    using Models.Dtos;

    public interface CommandService
    {
        Task Execute(string command, string buttonData, int messengerType, string messengerUsername);
    }

    public class CommandServiceImpl : CommandService
    {
        private const int CommandFunctionType = 1;
        private readonly SettingService settingService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IFunctionHandlerFactory functionHandlerFactory;
        private readonly ILogger logger;
        private readonly ActionService actionService;

#pragma warning disable S107 // Methods should not have too many parameters

        public CommandServiceImpl(
            SettingService settingService,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            IMessageLogger messageLogger,
            IChatbotConfigs chatbotConfigs,
            IFunctionHandlerFactory functionHandlerFactory,
            ILogger logger,
            ActionService actionService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.settingService = settingService;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.messageLogger = messageLogger;
            this.chatbotConfigs = chatbotConfigs;
            this.functionHandlerFactory = functionHandlerFactory;
            this.logger = logger;
            this.actionService = actionService;
        }

        public async Task Execute(
            string command,
            string buttonData,
            int messengerType,
            string messengerUsername)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var encodedCommand = WebUtility.HtmlEncode(command);

            var messengerDto = await settingService
                .GetMessenger(messengerType, messengerUsername)
                .ConfigureAwait(false);

            if (!await IsMessengerValid(messengerDto, messengerType, messengerUsername).ConfigureAwait(false))
            {
                return;
            }

            var user = await settingService
                .GetUser(messengerDto.UserId)
                .ConfigureAwait(false)
                    ?? throw new InvalidOperationException($"user cannot be found for messenger {messengerUsername}");

            var messenger = new Messenger(messengerType, messengerUsername);

            await RunCommand(encodedCommand, buttonData, user, messenger).ConfigureAwait(false);
        }

        private async Task RunCommand(
            string encodedCommand,
            string buttonData,
            User user,
            Messenger messenger)
        {
            try
            {
                var commandFunction = await GetCommandFunction(user.Id, encodedCommand).ConfigureAwait(false);

                await Task.WhenAll(
                    messageLogger.LogMessage(
                        user.Id,
                        CommonId.BotUserId,
                        messenger,
                        encodedCommand,
                        functionId: commandFunction?.Id ?? chatbotConfigs.NotRecognizedCommandFunctionId),
                    HandleCommand(encodedCommand, buttonData, user, messenger, commandFunction)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LogError(encodedCommand, messenger, user, ex).ConfigureAwait(false);
            }
        }

        private async Task<bool> IsMessengerValid(MessengerDto messengerDto, int messengerType, string messengerUsername)
        {
            if (messengerDto == null
                || !messengerDto.Enabled
                || messengerDto.IsVerifying)
            {
                await SendServiceUnavailableMessage(messengerType, messengerUsername).ConfigureAwait(false);
                logger.Info($"messenger {messengerUsername} is not verified or enabled");

                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await Task.FromResult(true).ConfigureAwait(false);
        }

        private async Task<Function> GetCommandFunction(int userId, string command)
        {
            var userFuntions = await settingService.GetFunctions(userId).ConfigureAwait(false);
            var commandFunction = userFuntions
                                    .FirstOrDefault(
                                        function => !string.IsNullOrWhiteSpace(command)
                                                    && !string.IsNullOrWhiteSpace(function.Command)
                                                    && command.IndexOf(function.Command, StringComparison.OrdinalIgnoreCase) == 0
                                        && function.Enabled);

            return commandFunction;
        }

        private async Task HandleCommand(
            string encodedCommand,
            string buttonData,
            User user,
            Messenger messenger,
            Function function)
        {
            if (function != null)
            {
                await HandleCommand(encodedCommand, user, messenger, function).ConfigureAwait(false);
            }
            else
            {
                await HandleNotRecognizedCommandMessage(user, messenger, encodedCommand, buttonData).ConfigureAwait(false);
            }
        }

        private async Task HandleCommand(
            string encodedCommand,
            User user,
            Messenger messenger,
            Function commandFunction)
        {
            var splittedCommand = SplitCommand(encodedCommand);
            var functionParam = new FunctionParam(
                    user,
                    commandFunction,
                    messenger,
                    new Dictionary<string, object>
                    {
                        ["command"] = splittedCommand.Item1,
                        ["parameters"] = splittedCommand.Item2
                    });

            await functionHandlerFactory
                .CreateFunctionHandler(commandFunction.Id)
                .Handle(functionParam)
                .ConfigureAwait(false);

            await actionService.ResetUserAction(user).ConfigureAwait(false);
        }

        private async Task LogError(string command, Messenger messenger, User user, Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Command Service Exception",
                                    $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                                    $"Messenger: {messenger.Username}, Type: {messenger.Type}",
                                    $"Command: {command}",
                                    ex.ToString());

            logger.Error(logMessage);

            if (chatbotConfigs.ReturnRawErrorToMessenger)
            {
                await messageSender.SendMessage(user.Id, messenger, logMessage.Replace("\r\n", string.Empty)).ConfigureAwait(false);
            }
            else
            {
                await SendErrorMessage(user, messenger).ConfigureAwait(false);
            }
        }

        private async Task HandleNotRecognizedCommandMessage(
            User user,
            Messenger messenger,
            string encodedCommand,
            string buttonData)
        {
            var userFunctions = await settingService.GetFunctions(user.Id).ConfigureAwait(false);

            if (!userFunctions.Any(func =>
                                        func.Type == CommandFunctionType
                                        && func.Enabled)
                || await actionService.NoRootActionExist().ConfigureAwait(false))
            {
                await SendNotRecognizedCommandMessage(user, messenger).ConfigureAwait(false);
                return;
            }

            var actionResult = await actionService.BuildAction(encodedCommand, buttonData, user, messenger).ConfigureAwait(false);

            await HandleActionResult(user, messenger, actionResult).ConfigureAwait(false);
        }

        private async Task HandleActionResult(User user, Messenger messenger, Actions.Models.ActionResult actionResult)
        {
            if (string.IsNullOrWhiteSpace(actionResult.Command))
            {
                await SendValidationMessage(user, messenger, actionResult).ConfigureAwait(false);

                await messageSender
                    .SendMessage(user.Id, messenger, actionResult.Message, actionResult.FunctionId)
                    .ConfigureAwait(false);
            }
            else
            {
                var function = (await settingService
                    .GetAllFunctions()
                    .ConfigureAwait(false))
                    .FirstOrDefault(fnc => fnc.Id == actionResult.FunctionId);

                await HandleCommand(actionResult.Command, user, messenger, function).ConfigureAwait(false);
            }
        }

        private async Task SendValidationMessage(User user, Messenger messenger, Actions.Models.ActionResult actionResult)
        {
            if (!string.IsNullOrWhiteSpace(actionResult.ValidationFailedMessage))
            {
                await messageSender
                    .SendMessage(user.Id, messenger, actionResult.ValidationFailedMessage, actionResult.FunctionId)
                    .ConfigureAwait(false);
            }
        }

        private Task SendNotRecognizedCommandMessage(User user, Messenger messenger)
            => messageSender.SendMessage(
                user.Id,
                messenger,
                messageBuilder.BuildNotRecognizedCommandMessage(user.Language),
                chatbotConfigs.NotRecognizedCommandFunctionId);

        private async Task SendErrorMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildErrorMessage(user.Language);

            await messageSender.SendMessage(user.Id, messenger, message).ConfigureAwait(false);
        }

        private async Task SendServiceUnavailableMessage(int messengerType, string messengerName)
        {
            var message = messageBuilder.BuildServiceIsUnavailableMessage(messengerType);

            await messageSender.SendMessage(new Messenger(messengerType, messengerName), message, logToDatabase: false).ConfigureAwait(false);
        }

        private static (string, string[]) SplitCommand(string command)
        {
            var firstSpaceIndex = command.IndexOf(" ", StringComparison.OrdinalIgnoreCase);

            if (firstSpaceIndex == -1)
            {
                return (command, new string[0]);
            }

            var commandName = command.Substring(0, firstSpaceIndex);
            var commandParam = command.Substring(firstSpaceIndex + 1).Split(" ");

            return (commandName, commandParam);
        }
    }
}