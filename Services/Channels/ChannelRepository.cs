using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Services.Channels
{
    public class ChannelRepository
    {
        private readonly ApplicationContext _context;

        public ChannelRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Channel>> GetAllChannelsAsync()
        {
            return await _context.Channels.ToListAsync();
        }

        public async Task<Channel?> GetChannelAsync(Guid id)
        {
            return await _context.Channels.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Channel?> GetChannelByTelegramIdAsync(long telegramId)
        {
            return await _context.Channels.SingleOrDefaultAsync(c => c.TelegramId == telegramId);
        }

        public void CreateChannel(Channel channel)
        {
            _context.Channels.Add(channel);
        }

        public void DeleteChannel(Channel channel)
        {
            _context.Channels.Remove(channel);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
