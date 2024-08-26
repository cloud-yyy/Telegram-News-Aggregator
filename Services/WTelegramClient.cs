using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using TL;
using WTelegram;

namespace Services
{
	public class WTelegramClient
	{
		private readonly ILogger _logger;

		private Client? _client;
		private User? _user;

		public Client? Client => _client;
		public bool LoggedIn => _client != null && _user != null;

		public WTelegramClient(ILogger<WTelegramClient> logger)
		{
			_logger = logger;
			Helpers.Log = (l, s) => {};
		}

		public async Task<bool> LoginAsync(MessageReaderUserDto userDto)
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
				_logger.LogInformation($"Logged in as: {_user.username} ({_user.ID})");
				return true;
			}
			else
			{
				_logger.LogError($"Login failed");
				return false;
			}
		}
	}
}
