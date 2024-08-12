using Shared.Dtos;

namespace Services.Contracts;

public interface ITagsExtractService
{
	public Task<MessageTagsDto> ExtractTagsAsync(MessageDto message);
}
