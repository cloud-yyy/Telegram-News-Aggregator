using TL;
using WTelegram;

namespace TelegramNewsAggregator
{
	public class WTelegramClient : ITelegramClient
	{
		private Client? _client;
		private UpdateManager? _updateManager;
		private User? _user;

		private readonly ILogger _logger;
        private readonly IMessageSerializer _messagesSerializer;
        private readonly HashSet<long> _listenedChannelsSet;


        public WTelegramClient(ILogger logger, IMessageSerializer messagesSerializer, IConfiguration configuration)
		{
			_logger = logger;
			_messagesSerializer = messagesSerializer;
			_listenedChannelsSet = configuration.GetSection("ListenedChannels").Get<HashSet<long>>();

			Helpers.Log = (l, s) => System.Diagnostics.Debug.WriteLine(s);
		}

		public async Task<bool> LoginAsync(UserDto userDto)
		{
			_client = new Client(paramName =>
			{
				return paramName switch
				{
					"api_id" => userDto.ApiId,
					"api_hash" => userDto.ApiHash,
					"phone_number" => userDto.PhoneNumber,
					"verification_code" => userDto.GetVerificationCode.Invoke(),
					_ => null
				};
			});
			
			_user = await _client.LoginUserIfNeeded();

			if (_user != null)
			{
				_logger.LogInfo($"Logged in as: {_user.username} ({_user.ID})");
				return true;
			}
			else
			{
				_logger.LogError($"Login failed");
				return false;
			}
		}

		public async Task HandleNewMessagesAsync()
		{
			if (_client == null)
				throw new UnauthorizedException();

			_updateManager = _client.WithUpdateManager(HandleUpdateAsync);
			var dialogs = await _client.Messages_GetAllDialogs();
			dialogs.CollectUsersChats(_updateManager.Users, _updateManager.Chats);
		}

		private async Task HandleUpdateAsync(Update update)
		{
			switch (update)
			{
				case UpdateNewMessage entity:
					await HandleMessageAsync(entity.message);
					break;
				case UpdateEditMessage entity:
					await HandleMessageAsync(entity.message);
					break;
				default:
					_logger.LogWarn($"Unhandled update type: {update.GetType()}");
					break;
			}
		}

		private async Task HandleMessageAsync(MessageBase messageBase, bool edited = false)
		{
			var channelId = messageBase.Peer.ID;
			var isListened = _listenedChannelsSet.Contains(channelId);

			if (isListened && messageBase is Message message)
			{
				_logger.LogInfo("New message handled");
				_updateManager.Chats.TryGetValue(channelId, out var chat);
				var channel = chat as Channel;

				var messageDto = new MessageDto(channelId, channel.Title, message.date, message.message, edited);
				_messagesSerializer.Serialize(messageDto);
			}
		}
	}
}
