using Entities.Exceptions;
using Repository;
using Shared.Dtos;
using TL;
using WTelegram;

namespace TelegramNewsAggregator;

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
		switch (update)
		{
			case UpdateNewMessage entity:
				await HandleMessageAsync(entity.message);
				break;
			default:
				// _logger.LogWarn($"Unhandled update type: {update.GetType()}");
				break;
		}
	}

	private async Task HandleMessageAsync(MessageBase messageBase)
	{
		var channelTelegramId = messageBase.Peer.ID;
		var listenedChannel = await _repository.GetChannelByTelegramIdAsync(channelTelegramId);
		var isListened = listenedChannel != null;

		if (isListened && messageBase is Message message)
		{
			_logger.LogInfo($"New message handled in thread: {Environment.CurrentManagedThreadId}");
			_updateManager!.Chats.TryGetValue(channelTelegramId, out var chat);

			var channel = chat as Channel;
			var messageDto = new MessageDto(Guid.NewGuid(), messageBase.ID, listenedChannel.Id, message.date, message.message);

			_broker.Push(messageDto);
			OnReceived?.Invoke(messageDto);
		}
	}
}
