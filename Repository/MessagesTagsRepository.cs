using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class MessagesTagsRepository
    {
        private readonly RepositoryContext _context;

        public MessagesTagsRepository(RepositoryContext context)
        {
            _context = context;
        }

        // public async Task<MessageTag> GetMessageTag(string name)
        // {
        //     return await _context.MessagesTags!.FirstOrDefaultAsync(t => t.Name == name);
        // }

        public void CreateMessageTag(MessageTag tag)
        {
            _context.MessagesTags!.Add(tag);
        }

        public void DeleteMessageTag(MessageTag tag)
        {
            _context.MessagesTags!.Remove(tag);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}