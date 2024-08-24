using AutoMapper;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace Services
{
    public class UserService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationContextFactory contextFactory, IMapper mapper)
        {
            _context = contextFactory.Create();
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            return _context.Users
                .Select(u => _mapper.Map<UserDto>(u))
                .AsEnumerable();
        }

        public async Task CreateUserAsync(UserDto userDto)
        {
            var existed = await _context.Users.SingleOrDefaultAsync(u => u.TelegramId == userDto.TelegramId);

            if (existed == null)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    TelegramId = userDto.TelegramId,
                    CreatedAt = DateTime.UtcNow,
                    SubscribtionsUpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasUserWithTelegramIdAsync(long telegramId)
        {
            return await _context.Users.AnyAsync(u => u.TelegramId == telegramId);
        }
    }
}