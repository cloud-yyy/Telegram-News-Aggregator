using Shared.Dtos;

namespace TelegramNewsAggregator;

public interface ITagsExtractService
{
	public Task<TagsForMessageDto> ExtractTagsAsync(MessageDto message);
}
