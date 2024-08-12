namespace Services.Contracts;

public interface ITelegramChannelIdResolver
{
	public Task<long> ResolveByTag(string tag);
}
