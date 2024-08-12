using Shared.Dtos;

namespace Services.Contracts
{
    public interface IMessageComparer
    {
        public Task<MessagesComparingResultDto> CompareByTags(IEnumerable<MessageTagsDto> messageTags);
    }
}
