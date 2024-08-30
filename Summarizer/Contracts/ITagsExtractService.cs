using Shared.Dtos;

namespace Summarizer.Contracts;

internal interface ITagsExtractService
{
	public Task<MessageTagsDto> ExtractTagsAsync(MessageDto message);
}
