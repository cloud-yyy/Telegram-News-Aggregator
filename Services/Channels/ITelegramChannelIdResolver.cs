namespace Services.Channels;

public interface ITelegramChannelIdResolver
{
	public Task<long> ResolveByTag(string tag);
}
