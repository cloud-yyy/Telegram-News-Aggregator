namespace TelegramNewsAggregator;

public interface ITelegramChannelIdResolver
{
	public Task<long> ResolveByTag(string tag);
}
