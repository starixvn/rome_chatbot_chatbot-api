namespace Chatbot.Modules.Funtions
{
    using System;
    using System.Collections.Generic;
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
        Task Execute(string command, int messengerType, string messengerUsername);
    }

    public class CommandServiceImpl : CommandService
    {
        private const int NotRecognizedCommandFunctionId = 1000000;
        private readonly SettingService settingService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly IFunctionHandler functionHandler;
        private readonly ILogger logger;

        public CommandServiceImpl(
            SettingService settingService,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            IMessageLogger messageLogger,
            IChatbotConfigs chatbotConfigs,
            IFunctionHandler commandHandler,
            ILogger logger)
        {
            this.settingService = settingService;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.logger = logger;
            this.chatbotConfigs = chatbotConfigs;
            this.functionHandler = commandHandler;
            this.messageLogger = messageLogger;
        }

        public async Task Execute(string command, int messengerType, string messengerUsername)
        {
            var messengerDto = await settingService.GetMessenger(messengerType, messengerUsername);

            if (messengerDto == null
                || !messengerDto.Enabled
                || messengerDto.IsVerifying)
            {
                throw new InvalidOperationException($"messenger {messengerUsername} is not verified or enabled");
            }

            var user = await settingService.GetUser(messengerDto.UserId)
                ?? throw new InvalidOperationException($"user cannot be found for messenger {messengerUsername}");

            var messenger = new Messenger(messengerType, messengerUsername);

            try
            {
                var commandFunction = await GetCommandFunction(messengerDto.UserId, command);

                await Task.WhenAll(
                    messageLogger.LogMessage(user.Id, CommonId.BotUserId, messenger, command, commandFunction?.Id ?? 0),
                    RunCommand(command, user, messenger, commandFunction));
            }
            catch (Exception ex)
            {
                await LogError(command, messengerDto, user, ex);
            }
        }

        private async Task<Function> GetCommandFunction(int userId, string command)
        {
            var userFuntions = await settingService.GetFunctions(userId);
            var commandFunction = userFuntions
                                    .FirstOrDefault(
                                        function => string.Compare(function.Command, command, true) == 0
                                        && function.Enabled);

            return commandFunction;
        }

        private async Task RunCommand(
            string command,
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
                            { "command", command }
                        });

                await functionHandler.Handle(functionParam);
            }
            else
            {
                await SendNotRecognizedCommandMessage(user, messenger);
            }
        }

        private async Task LogError(string command, MessengerDto messengerDto, User user, Exception ex)
        {
            var logMessage = string.Join(
                                    "\n",
                                    "Command Service Exception",
                                    $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                                    $"Messenger: {messengerDto.Username}, Type: {messengerDto.Type}",
                                    $"Command: {command}",
                                    ex.ToString());

            logger.Error(logMessage);

            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);

            if (chatbotConfigs.ReturnRawErrorToMessenger)
            {
                await messageSender.SendMessage(user.Id, messenger, logMessage.Replace("\r\n", string.Empty));
            }
            else
            {
                await SendErrorMessage(user, messenger);
            }
        }

        protected async Task SendNotRecognizedCommandMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildNotRecognizedCommandMessage(user.Language);

            await messageSender.SendMessage(user.Id, messenger, message, chatbotConfigs.NotRecognizedCommandFunctionId);
        }

        protected async Task SendErrorMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildErrorMessage(user.Language);

            await messageSender.SendMessage(user.Id, messenger, message);
        }
    }
}