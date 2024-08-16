using Entities.Exceptions;
using Services.Contracts;
using TL;

namespace Services;

public class WTelegramChannelIdResolver : ITelegramChannelIdResolver
{
    private readonly WTelegramClient _client;

    public WTelegramChannelIdResolver(WTelegramClient client)
	{
		_client = client;
	}

    public async Task<long> ResolveByTag(string tag)
    {
		if (!_client.LoggedIn)
			throw new UnauthorizedException();

		var resolvedPeer = await _client.Client.Contacts_ResolveUsername(tag.Trim());

		if (resolvedPeer.peer is PeerChannel peerChannel)
		{
			long id = peerChannel.channel_id;
			return id;
		}
		else
		{
			throw new ChannelNotFoundException(tag);
		}
	}
}
