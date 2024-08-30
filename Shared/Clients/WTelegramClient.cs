using Microsoft.Extensions.Logging;
using Shared.Dtos;
using TL;
using WTelegram;

namespace Shared.Clients;

public class WTelegramClient
{
	private readonly ILogger<WTelegramClient> _logger;
	private readonly MessageReaderUserDto _userDto;

	private Client? _client;
	private User? _user;

	public Client? Client => _client;
	public bool LoggedIn => _client != null && _user != null;

	public WTelegramClient(ILogger<WTelegramClient> logger, MessageReaderUserDto userDto)
	{
		_logger = logger;
		_userDto = userDto;
		Helpers.Log = (l, s) => { };
	}

	public async Task<bool> LoginAsync()
	{
		_client = new Client(paramName =>
		{
			return paramName switch
			{
				"api_id" => _userDto.ApiId,
				"api_hash" => _userDto.ApiHash,
				"phone_number" => _userDto.PhoneNumber,
				"verification_code" => _userDto.GetVerificationCode.Invoke(),
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
