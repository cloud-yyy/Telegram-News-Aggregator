using Entities.Exceptions;
using Repository;
using Services.Contracts;
using Shared.Dtos;
using TL;
using WTelegram;

namespace Services;

public class WTelegramMessageReader : ITelegramMessageReader
{
    private readonly WTelegramClient _client;
    private readonly MessageBroker _broker;
    private readonly ILogger _logger;
    private readonly ChannelRepository _repository;
    private UpdateManager? _updateManager;

    public event Action<MessageDto>? OnReceived;

    public WTelegramMessageReader(WTelegramClient client, MessageBroker broker, ChannelRepository repository, ILogger logger)
	{
		_client = client;
		_broker = broker;
		_logger = logger;
		_repository = repository;
	}

	public async Task StartListeningAsync()
	{
		if (!_client.LoggedIn)
			throw new UnauthorizedException();

		_updateManager = _client.Client.WithUpdateManager(HandleUpdateAsync);
		var dialogs = await _client.Client!.Messages_GetAllDialogs();
		dialogs.CollectUsersChats(_updateManager.Users, _updateManager.Chats);
	}

	private async Task HandleUpdateAsync(Update update)
	{
		if (update is UpdateNewMessage entity)
			await HandleMessageAsync(entity.message);
	}

	private async Task HandleMessageAsync(MessageBase messageBase)
	{
		var channelTelegramId = messageBase.Peer.ID;
		var listenedChannel = await _repository.GetChannelByTelegramIdAsync(channelTelegramId);
		var isListened = listenedChannel != null;

		if (isListened && messageBase is Message message)
		{
			_logger.LogInfo($"New message handled in thread: {Environment.CurrentManagedThreadId}");

			var messageDto = new MessageDto
			(
				id: Guid.NewGuid(), 
				telegramId: messageBase.ID, 
				channelId: listenedChannel!.Id, 
				sendedAt: message.date, 
				content: message.message
			);

			_broker.Push(messageDto);
			OnReceived?.Invoke(messageDto);
		}
	}
}
