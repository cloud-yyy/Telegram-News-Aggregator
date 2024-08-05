using Shared.Dtos;

namespace TelegramNewsAggregator;

public interface ITelegramMessageReader
{
	public Task StartListeningAsync();
	public event Action<MessageDto> OnReceived;
}
