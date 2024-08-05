namespace Shared.Dtos
{
    public record TagsForMessageDto(Guid MessageId, IEnumerable<TagDto>  Tags);
}
