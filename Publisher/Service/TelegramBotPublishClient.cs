using System.Text;
using Entities.Exceptions;
using MessageBroker.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Publisher.Contracts;
using Repository;
using Shared.Dtos;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Publisher
{
    internal class TelegramBotPublishClient : IPublishClient, IMessageConsumer<SummaryDto>
    {
        private readonly TelegramBotClient _botClient;
        private readonly CancellationTokenSource _connectionCancellationTokenSource;
        private readonly ApplicationContext _context;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;

        public TelegramBotPublishClient(
            ILogger<TelegramBotPublishClient> logger, 
            IDbContextFactory<ApplicationContext> contextFactory)
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

            _context = contextFactory.CreateDbContext();
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Notify(SummaryDto message)
        {
            await Publish(message);
        }

        public async Task Publish(SummaryDto message)
        {
            await _semaphore.WaitAsync();

            try
            {
                _logger.LogInformation($"Sending message started in thread: {Environment.CurrentManagedThreadId}");

                var renderedMessage = RenderMessage(message);
                var users = await _context.Users.ToListAsync();

                foreach (var user in users)
                    await SendMessageAsync(user.TelegramId, renderedMessage);

                _logger.LogInformation($"Sending message completed in thread: {Environment.CurrentManagedThreadId}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            // TODO: When update received while Publish executing, it causes DbContext exception

            if (update.Message != null && update.Message.Text == "/start")
            {
                var chatId = update.Message.Chat.Id;

                if (!_context.Users.Any(u => u.TelegramId == chatId))
                {
                    var user = new Entities.Models.User()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        SubscribtionsUpdatedAt = DateTime.UtcNow,
                        TelegramId = chatId
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await SendMessageAsync(chatId, "Succesefully logged in. You have started receiving news!");
                }
                else
                {
                    await SendMessageAsync(chatId, "You already subscribed on news!");
                }

                _logger.LogInformation($"Message received from chat with id {chatId}");
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

        private static string RenderMessage(SummaryDto dto)
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
