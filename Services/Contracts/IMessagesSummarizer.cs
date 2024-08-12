using Shared.Dtos;

namespace Services.Contracts
{
    public interface IMessagesSummarizer
    {
        public Task<SummaryDto> Summarize(List<MessageDto> messages);
    }
}
