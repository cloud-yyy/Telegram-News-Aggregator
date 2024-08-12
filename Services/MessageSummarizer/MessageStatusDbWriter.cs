using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Services
{
    public class MessageStatusDbWriter
    {
        private readonly RepositoryContext _context;
        private readonly SemaphoreSlim _semaphore;

        public MessageStatusDbWriter(RepositoryContextFactory contextFactory)
        {
            _context = contextFactory.Create();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task SetSummarizedSingleAsync(IEnumerable<Guid> ids)
        {
            await SetStatusAsync(ids, Message.SummarizationStatus.SummarizedSingle);
        }

        public async Task SetSummarizedMultipleAsync(IEnumerable<Guid> ids)
        {
            await SetStatusAsync(ids, Message.SummarizationStatus.SummarizedMultiple);
        }

        private async Task SetStatusAsync(IEnumerable<Guid> ids, Message.SummarizationStatus status)
        {
            await _context.Messages
                .Where(m => ids.Contains(m.Id))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(m => m.Status, status));

            await _context.SaveChangesAsync();
        }
    }
}