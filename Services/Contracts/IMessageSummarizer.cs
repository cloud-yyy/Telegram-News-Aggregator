using Shared.Dtos;

namespace Services.Contracts
{
    public interface IMessagesSummarizer
    {
        public Task<SummaryDto> SummarizeAsync(IEnumerable<MessageDto> messages);
    }
}