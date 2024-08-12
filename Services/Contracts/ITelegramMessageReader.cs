using Shared.Dtos;

namespace Services.Contracts;

public interface ITelegramMessageReader
{
	public Task StartListeningAsync();
	public event Action<MessageDto> OnReceived;
}
