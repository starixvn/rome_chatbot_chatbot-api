namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Constants;
    using Chatbot.Modules.Funtions.FunctionHandler;
    using Chatbot.Modules.Funtions.Params;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings;
    using Fanex.Logging;
    using Models;
    using Models.Dtos;

    public interface CommandService
    {
        Task Execute(string command, string parameter, int messengerType, string messengerUsername);
    }

    public class CommandServiceImpl : CommandService
    {
        private readonly SettingService settingService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IFunctionHandlerFactory functionHandlerFactory;
        private readonly ILogger logger;

        public CommandServiceImpl(
            SettingService settingService,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            IMessageLogger messageLogger,
            IChatbotConfigs chatbotConfigs,
            IFunctionHandlerFactory functionHandlerFactory,
            ILogger logger)
        {
            this.settingService = settingService;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.messageLogger = messageLogger;
            this.chatbotConfigs = chatbotConfigs;
            this.functionHandlerFactory = functionHandlerFactory;
            this.logger = logger;
        }

        public async Task Execute(string command, string parameter, int messengerType, string messengerUsername)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var encodedCommand = System.Net.WebUtility.HtmlEncode(command);
            var encodedParameter = System.Net.WebUtility.HtmlEncode(parameter);

            var messengerDto = await settingService.GetMessenger(messengerType, messengerUsername).ConfigureAwait(false);

            if (messengerDto == null
                || !messengerDto.Enabled
                || messengerDto.IsVerifying)
            {
                await SendServiceUnavailableMessage(messengerType, messengerUsername).ConfigureAwait(false);
                logger.Info($"messenger {messengerUsername} is not verified or enabled");
                return;
            }

            var user = await settingService.GetUser(messengerDto.UserId).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"user cannot be found for messenger {messengerUsername}");

            var messenger = new Messenger(messengerType, messengerUsername);

            try
            {
                var commandFunction = await GetCommandFunction(messengerDto.UserId, encodedCommand).ConfigureAwait(false);

                await Task.WhenAll(
                    messageLogger.LogMessage(user.Id, CommonId.BotUserId, messenger, $"{encodedCommand} {encodedParameter}", functionId: commandFunction?.Id ?? chatbotConfigs.NotRecognizedCommandFunctionId),
                    RunCommand(encodedCommand, encodedParameter, user, messenger, commandFunction)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await LogError(encodedCommand, encodedParameter, messengerDto, user, ex).ConfigureAwait(false);
            }
        }

        private async Task<Function> GetCommandFunction(int userId, string command)
        {
            var userFuntions = await settingService.GetFunctions(userId).ConfigureAwait(false);
            var commandFunction = userFuntions
                                    .FirstOrDefault(
                                        function => string.Compare(function.Command, command, true, CultureInfo.InvariantCulture) == 0
                                        && function.Enabled);

            return commandFunction;
        }

        private async Task RunCommand(
            string command,
            string parameter,
            User user,
            Messenger messenger,
            Function commandFunction)
        {
            if (commandFunction != null)
            {
                var functionParam = new FunctionParam(
                        user,
                        commandFunction,
                        messenger,
                        new Dictionary<string, object>
                        {
                            [nameof(command)] = command,
                            [nameof(parameter)] = parameter
                        });

                await functionHandlerFactory
                    .CreateFunctionHandler(commandFunction.Id)
                    .Handle(functionParam)
                    .ConfigureAwait(false);
            }
            else
            {
                await SendNotRecognizedCommandMessage(user, messenger).ConfigureAwait(false);
            }
        }

        private async Task LogError(string command, string parameter, MessengerDto messengerDto, User user, Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Command Service Exception",
                                    $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                                    $"Messenger: {messengerDto.Username}, Type: {messengerDto.Type}",
                                    $"Command: {command}",
                                    $"Parameter: {parameter}",
                                    ex.ToString());

            logger.Error(logMessage);

            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);

            if (chatbotConfigs.ReturnRawErrorToMessenger)
            {
                await messageSender.SendMessage(user.Id, messenger, logMessage.Replace("\r\n", string.Empty)).ConfigureAwait(false);
            }
            else
            {
                await SendErrorMessage(user, messenger).ConfigureAwait(false);
            }
        }

        private async Task SendNotRecognizedCommandMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildNotRecognizedCommandMessage(user.Language);

            await messageSender.SendMessage(user.Id, messenger, message, chatbotConfigs.NotRecognizedCommandFunctionId).ConfigureAwait(false);
        }

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
    }
}