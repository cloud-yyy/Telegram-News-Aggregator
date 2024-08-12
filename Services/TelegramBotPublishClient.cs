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
        private readonly List<long> _followerChatIds;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;

        public TelegramBotPublishClient(ILogger logger)
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
            _semaphore = new SemaphoreSlim(1, 1);
            _followerChatIds = FileReaderWriterDebug.ReadFollowerChatIds();
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

            foreach (var chatId in _followerChatIds)
            {
                await _botClient.SendTextMessageAsync
                (
                    chatId: chatId,
                    text: renderedMessage,
                    parseMode: ParseMode.Html,
                    cancellationToken: _connectionCancellationTokenSource.Token
                );
            }

            _logger.LogInfo($"Sending message completed in thread: {Environment.CurrentManagedThreadId}");
            _semaphore.Release();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message != null && update.Message.Text == "/start")
            {
                var chatId = update.Message.Chat.Id;

                if (!_followerChatIds.Contains(chatId))
                {
                    _followerChatIds.Add(chatId);
                    await FileReaderWriterDebug.AddFollowerChatIdAsync(chatId);
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

        private string RenderMessage(SummaryDto dto)
        {
            var builder = new StringBuilder();

            builder
                .AppendLine($"<b>{dto.Title}</b>\n")
                .AppendLine($"{dto.Content}\n")
                .AppendLine($"Source: {dto.Sources.First()}\n")
                .AppendLine($"<i>Message was generated using AI, so don't trust it completely.</i>");

            return builder.ToString();
        }
    }
}
