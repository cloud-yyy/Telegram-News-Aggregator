using AutoMapper;
using Entities.Exceptions;
using Entities.Models;
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

        public async Task SubscribeOnChannel(long userTelegramId, ChannelDto channel)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.TelegramId == userTelegramId);

            if (user == null)
                throw new UserNotFoundException(userTelegramId);

            var userChannel = new UserChannel()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ChannelId = channel.Id
            };

            var existed = _context.UserChannels
                .Where(uc => uc.UserId == user.Id && uc.ChannelId == channel.Id);

            if (!existed.Any())
            {
                _context.UserChannels.Add(userChannel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnsubscribeOfChannel(long userTelegramId, ChannelDto channel)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.TelegramId == userTelegramId);

            if (user == null)
                throw new UserNotFoundException(userTelegramId);
                
            var existed = _context.UserChannels
                .SingleOrDefault(uc => uc.UserId == user.Id && uc.ChannelId == channel.Id);

            if (existed != null)
            {
                _context.UserChannels.Remove(existed);
                await _context.SaveChangesAsync();
            }
        }
    }
}
