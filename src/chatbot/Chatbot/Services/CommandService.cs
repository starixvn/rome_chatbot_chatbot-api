namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Constants;
    using Microsoft.Extensions.Logging;
    using Models;

    public interface CommandService
    {
        Task Execute(string command, int messengerType, string messengerUsername);
    }

    public class CommandServiceImpl : CommandService
    {
        private const int OutstandingFunctionId = 1;
        private const int WinlossFunctionId = 2;
        private const int BalanceFunctionId = 3;
        private readonly ILogger<CommandServiceImpl> logger;

        private static readonly Dictionary<int, ICommandHandler> commandHandlerTypes = new Dictionary<int, ICommandHandler>
        {
            { OutstandingFunctionId, new OutstandingCommandHandler() },
            { WinlossFunctionId, new WinLossCommandHandler() },
            { BalanceFunctionId, new BalanceInfoCommandHandler() }
        };

        private readonly SettingService settingService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;

        public CommandServiceImpl(
            SettingService settingService,
            IMessageSender messageSender,
            IMessageBuilder messageBuilder,
            IMessageLogger messageLogger,
            ILogger<CommandServiceImpl> logger)
        {
            this.settingService = settingService;
            this.messageSender = messageSender;
            this.messageBuilder = messageBuilder;
            this.messageLogger = messageLogger;
            this.logger = logger;
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

            var user = await settingService.GetUser(messengerDto.UserId);

            if (user == null)
            {
                throw new InvalidOperationException($"user cannot be found for messenger {messengerUsername}");
            }

            try
            {
                var userFuntions = await settingService.GetFunctions(messengerDto.UserId);
                var messenger = new Messenger(messengerType, messengerUsername);
                var commandFunction = userFuntions
                                        .FirstOrDefault(
                                            function => string.Compare(function.Command, command, true) == 0
                                            && function.Enabled);

                await messageLogger.LogMessage(user.Id, CommonId.BotUserId, messenger, command);

                if (commandFunction != null
                    && commandHandlerTypes.ContainsKey(commandFunction.Id))
                {
                    await commandHandlerTypes[commandFunction.Id].RunCommand(
                            user,
                            messenger,
                            new Dictionary<string, object>
                            {
                            { "command", command }
                            });
                }
                else
                {
                    await SendNotRecognizedCommandMessage(user, messenger);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    string.Join(
                        "\r\n",
                        "Command Service Exception",
                        $"User: {user.Id}, CustId: {user.CustId}, SubAccId: {user.SubAccountId}, Language: {user.Language}",
                        $"Messenger: {messengerDto.Username}, Type: {messengerDto.Type}",
                        $"Command: {command}",
                        ex.Message
                    ));
            }
        }

        protected async Task SendNotRecognizedCommandMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildNotRecognizedCommandMessage();

            await SendMessage(user, messenger, message);
        }

        protected async Task SendErrorMessage(User user, Messenger messenger)
        {
            var message = messageBuilder.BuildErrorMessage();

            await SendMessage(user, messenger, message);
        }

        private async Task SendMessage(User user, Messenger messenger, string message)
        {
            await messageSender.SendMessage(messenger, message);
            await messageLogger.LogMessage(CommonId.BotUserId, user.Id, messenger, message);
        }
    }
}