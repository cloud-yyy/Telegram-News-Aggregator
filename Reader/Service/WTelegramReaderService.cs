using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reader.Contracts;
using Repository;
using Shared.Clients;
using Shared.Dtos;
using TL;
using WTelegram;

namespace Reader.Service;

internal class WTelegramReaderService : IReaderService
{
    private readonly WTelegramClient _client;
    private readonly ILogger _logger;
    private readonly IMessageSaver _messageSaver;
    private readonly ApplicationContext _context;
    private UpdateManager? _updateManager;

    public WTelegramReaderService(
		IDbContextFactory<ApplicationContext> contextFactory,
		IMessageSaver messageSaver,
		WTelegramClient client,
		ILogger<WTelegramReaderService> logger)
	{
		_client = client;
		_logger = logger;
		_messageSaver = messageSaver;
		_context = contextFactory.CreateDbContext();
	}

	public async Task StartReadingAsync()
	{
		if (!_client.LoggedIn)
			await _client.LoginAsync();

		_updateManager = _client.Client.WithUpdateManager(HandleUpdateAsync);
		var dialogs = await _client.Client!.Messages_GetAllDialogs();
		dialogs.CollectUsersChats(_updateManager.Users, _updateManager.Chats);
	}

	public Task StopReadingAsync()
	{
		_updateManager = null;
		return Task.CompletedTask;
	}

	private async Task HandleUpdateAsync(Update update)
	{
		if (update is UpdateNewMessage entity)
		{
			var messageBase = entity.message;
			var channelTelegramId = messageBase.Peer.ID;

			var listenedChannel = await _context.Channels
				.SingleOrDefaultAsync(c => c.TelegramId == channelTelegramId);
			
			var isListened = listenedChannel != null;

			if (isListened && messageBase is Message message)
			{
				_logger.LogInformation($"New message handled in thread: {Environment.CurrentManagedThreadId}");

				var messageDto = new MessageDto
				(
					id: Guid.NewGuid(),
					telegramId: messageBase.ID,
					channelId: listenedChannel!.Id,
					sendedAt: message.date,
					content: message.message,
					uri: BuildUri(listenedChannel, messageBase.ID)
				);

				_messageSaver.Save(messageDto);
			}
		}
	}

	private static string BuildUri(Entities.Models.Channel channel, long messageId)
	{
		return channel.IsPrivate ? channel.Name : $"t.me/{channel.Name}/{messageId}";
	}
}
