using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class MessageRepository
    {
        private readonly RepositoryContext _context;

        public MessageRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _context.Messages!.ToListAsync();
        }

        public async Task<Message?> GetMessageAsync(Guid id)
        {
            return await _context.Messages!.SingleOrDefaultAsync(m => m.Id == id);
        }

        public void CreateMessage(Message message)
        {
            _context.Messages!.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages!.Remove(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
