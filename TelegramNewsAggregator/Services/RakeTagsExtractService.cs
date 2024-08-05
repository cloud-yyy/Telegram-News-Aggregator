using Entities;
using Shared.Dtos;

namespace TelegramNewsAggregator;

public class RakeTagsExtractService : ITagsExtractService, IMessageConsumer<MessageDto>
{
    private readonly Rake.Rake _rake;
    private readonly MessageBroker _broker;

    public RakeTagsExtractService(MessageBroker broker)
	{
		_rake = new Rake.Rake();
		_broker = broker;
	}

	public Task Notify(MessageDto message)
	{
		return ExtractTagsAsync(message);
	}
	
	public Task<TagsForMessageDto> ExtractTagsAsync(MessageDto message)
    {
		var text = message.Content;

		if (string.IsNullOrEmpty(text))
			throw new ArgumentException($"Cannot extract tags from null-valued or empty string {nameof(text)}");

		var tags = _rake
			.Run(text).Keys
			.Select(t => new TagDto(t));

		var tagsForMessage = new TagsForMessageDto(message.Id, tags);

		_broker.Push(tagsForMessage);

		return Task.FromResult(tagsForMessage);
    }
}
