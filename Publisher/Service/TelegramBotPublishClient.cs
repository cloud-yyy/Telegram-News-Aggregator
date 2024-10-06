using System.Text;
using System.Threading.Channels;
using Entities.Exceptions;
using MessageBroker.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;
        private readonly string _botTag;

        public TelegramBotPublishClient(
            ILogger<TelegramBotPublishClient> logger, 
            IDbContextFactory<ApplicationContext> contextFactory,
            IConfiguration configuration)
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

            _contextFactory = contextFactory;
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
            _botTag = configuration.GetValue<string>("BotTag")!;
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
                using var context = _contextFactory.CreateDbContext();

                _logger.LogInformation($"Sending message started in thread: {Environment.CurrentManagedThreadId}");

                var renderedMessage = RenderMessage(message);
                var userIds = await GetRecipientsIds(message.Sources.Select(m => m.ChannelId), context);

                foreach (var userId in userIds)
                    await SendMessageAsync(userId, renderedMessage);

                _logger.LogInformation($"Sending message completed in thread: {Environment.CurrentManagedThreadId}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<IEnumerable<long>> GetRecipientsIds(IEnumerable<Guid> sourcesIds, ApplicationContext context)
        {
            // по итогу нужно список всех юзеров, кому отправить это сообщение

            // есть сообщение, у него есть источники. у источников есть subscribers, 
            // во первых пользователей тянем оттуда.

            // var channelSubscribers = await context.Users
            //     .Include(t => t.Subscribtions)
            //     .Where(u =>
            //         u.Subscribtions.Select(s => s.Id).Intersect(
            //             sourcesIds
            //         )
            //         .ToList()
            //         .Any())
            //     .Select(u => u.TelegramId)
            //     .ToListAsync();

            // во вторых эти источники есть в топиках, то есть нужно получить топики
            // из источников и оттуда второй список подпищиков

            var topics = await context.Topics
                .Include(t => t.Subscribers)
                .Where(t => t.Channels!
                    .Select(s => s.Id)
                    .Intersect(sourcesIds)
                    .Any())
                .ToListAsync();

            var topicsSubscribers = topics
                .SelectMany(t => t.Subscribers!)
                .Select(u => u.TelegramId)
                .Distinct()            // remove only this line while using channels subscribers
                .ToList();

            // потом объединяем эти два списка и делаем дистинкт
            
            // var result = channelSubscribers
            //     .Union(topicsSubscribers)
            //     .Distinct();

            return topicsSubscribers;
        }


        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            using var context = _contextFactory.CreateDbContext();

            if (update.Message != null && update.Message.Text == "/start")
            {
                var chatId = update.Message.Chat.Id;

                if (!context.Users.Any(u => u.TelegramId == chatId))
                {
                    var user = new Entities.Models.User()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        SubscribtionsUpdatedAt = DateTime.UtcNow,
                        TelegramId = chatId
                    };

                    context.Users.Add(user);
                    await context.SaveChangesAsync(token);

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

        private string RenderMessage(SummaryDto dto)
        {
            var builder = new StringBuilder();

            builder
                .AppendLine($"<b>{dto.Title}</b>\n")
                .AppendLine($"{dto.Content}\n");

            var uris = dto.Sources.Select(s => s.Uri).Distinct().ToList();

            foreach (var uri in uris)
                builder.AppendLine($"<i><a href='{uri}'>Перейти к источнику</a></i>");
            
            builder
                .AppendLine(_botTag)
                .AppendLine($"\n<i>Message was generated using AI</i>");

            return builder.ToString();
        }
    }
}
