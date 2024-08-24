using System.Text;
using Entities.Exceptions;
using Services.Contracts;
using Shared.Dtos;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Services
{
    public class TelegramBotPublishClient : IPublishClient, IMessageConsumer<SummaryDto>
    {
        private readonly TelegramBotClient _botClient;
        private readonly CancellationTokenSource _connectionCancellationTokenSource;
        private readonly ILogger _logger;
        private readonly UserService _userService;
        private readonly SemaphoreSlim _semaphore;

        public TelegramBotPublishClient(ILogger logger, UserService userService)
        {
            var token = Environment.GetEnvironmentVariable("bot_token");

            if (token == null)
                throw new EnviromentVariableNotFoundException("bot_token");
            
            _botClient = new TelegramBotClient(token);
            _connectionCancellationTokenSource = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = [UpdateType.Message]
            };

            _botClient.StartReceiving
            (
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                receiverOptions,
                _connectionCancellationTokenSource.Token
            );

            _logger = logger;
            _userService = userService;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Notify(SummaryDto message)
        {
            _logger.LogInfo($"Publisher notified in thread: {Environment.CurrentManagedThreadId}");
            await Publish(message);
        }

        public async Task Publish(SummaryDto message)
        {
            await _semaphore.WaitAsync();
            _logger.LogInfo($"Sending message started in thread: {Environment.CurrentManagedThreadId}");

            var renderedMessage = RenderMessage(message);
            var users = await _userService.GetAllUsers();

            foreach (var user in users)
                await SendMessageAsync(user.TelegramId, renderedMessage);

            _logger.LogInfo($"Sending message completed in thread: {Environment.CurrentManagedThreadId}");
            _semaphore.Release();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message != null && update.Message.Text == "/start")
            {
                var chatId = update.Message.Chat.Id;

                if (!await _userService.HasUserWithTelegramIdAsync(chatId))
                {
                    var user = new UserDto(chatId);
                    await _userService.CreateUserAsync(user);
                    await SendMessageAsync(chatId, "Succesefully logged in. You have started receiving news!");
                }
                else
                {
                    await SendMessageAsync(chatId, "You already subscribed on news!");
                }

                _logger.LogInfo($"Message received from chat with id {chatId}");
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task SendMessageAsync(long chatId, string text)
        {
            await _botClient.SendTextMessageAsync
            (
                chatId: chatId,
                text: text,
                parseMode: ParseMode.Html,
                cancellationToken: _connectionCancellationTokenSource.Token
            );
        }

        private string RenderMessage(SummaryDto dto)
        {
            var builder = new StringBuilder();

            builder
                .AppendLine($"<b>{dto.Title}</b>\n")
                .AppendLine($"{dto.Content}\n")
                .AppendLine($"Sources:");

            foreach (var uri in dto.Sources.Select(s => s.Uri).Distinct())
                builder.AppendLine($"<i>{uri}</i>");
            
            builder
                .AppendLine($"\n<i>Message was generated using AI</i>");

            return builder.ToString();
        }
    }
}
