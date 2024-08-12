namespace Shared.Dtos
{
    public record MessageTagsDto(Guid MessageId, IEnumerable<string> Tags);
}
