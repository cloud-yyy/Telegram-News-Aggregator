using AutoMapper;
using Entities.Exceptions;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace Services.Subscribtions
{
    public class SubscribtionsService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public SubscribtionsService(IDbContextFactory<ApplicationContext> contextFactory, IMapper mapper)
        {
            _context = contextFactory.CreateDbContext();
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChannelDto>> GetChannels()
        {
            var channels = _context.Channels
                .Select(c => _mapper.Map<ChannelDto>(c))
                .AsEnumerable();

            return await Task.FromResult(channels);
        }

        public async Task SubscribeOnChannel(long userTelegramId, long channelTelegramId)
        {
            var user = await _context.Users
                .Include(u => u.Subscribtions)
                .AsSplitQuery()
                .SingleOrDefaultAsync(u => u.TelegramId == userTelegramId);

            if (user == null)
                throw new UserNotFoundException(userTelegramId);

            var channel = _context.Channels
                .SingleOrDefault(c => c.TelegramId == channelTelegramId);

            if (channel == null)
                throw new ChannelNotFoundException(channelTelegramId);

            if (!user.Subscribtions.Any(c => c.Id == channel.Id))
                user.Subscribtions.Add(channel);

            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeOfChannel(long userTelegramId, long channelTelegramId)
        {
            var user = await _context.Users
                .Include(u => u.Subscribtions)
                .AsSplitQuery()
                .SingleOrDefaultAsync(u => u.TelegramId == userTelegramId);

            if (user == null)
                throw new UserNotFoundException(userTelegramId);

            var subscribedChannel = user.Subscribtions
                .SingleOrDefault(s => s.TelegramId == channelTelegramId);

            if (subscribedChannel != null)
            {
                user.Subscribtions.Remove(subscribedChannel);
                await _context.SaveChangesAsync();
            }
        }
    }
}
