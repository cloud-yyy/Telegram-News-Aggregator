using Entities.Models;
using Repository;

namespace Services
{
    public class MessageDbReader
    {
        private readonly RepositoryContext _context;
        private readonly SemaphoreSlim _semaphore;

        public MessageDbReader(RepositoryContextFactory contextFactory)
        {
            _context = contextFactory.Create();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<Message?> GetMessageAsync(Guid id)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await _context.Messages.FindAsync(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
