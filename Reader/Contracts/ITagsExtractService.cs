using Shared.Dtos;

namespace Reader.Contracts
{
    internal interface ITagsExtractService
    {
        public Task<IEnumerable<string>> ExtractTagsAsync(MessageDto message);
    }
}