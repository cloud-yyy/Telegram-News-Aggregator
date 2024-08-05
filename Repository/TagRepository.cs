using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TagRepository
    {
        private readonly RepositoryContext _context;

        public TagRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetTagAsync(Guid id)
        {
            return await _context.Tags!.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _context.Tags!.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<bool> HasTagWithNameAsync(string name)
        {
            var tag = await GetTagByNameAsync(name);
            return tag != null;
        }

        public void CreateTag(Tag tag)
        {
            _context.Tags!.Add(tag);
        }

        public void DeleteTag(Tag tag)
        {
            _context.Tags!.Remove(tag);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
