using Shared.Dtos;

namespace Summarizer.Contracts;

public interface IMessagesSummarizer
{
    public Task<SummaryDto> SummarizeAsync(IEnumerable<MessageDto> messages);
}