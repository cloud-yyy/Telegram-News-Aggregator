using AutoMapper;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace Services.Users
{
    public class UserService
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;
        private readonly IMapper _mapper;

        public UserService(IDbContextFactory<ApplicationContext> contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public List<UserDto> GetAllUsers()
        {
            using var context = _contextFactory.CreateDbContext();

            return context.Users
                .Select(u => _mapper.Map<UserDto>(u))
                .ToList();
        }

        public async Task<UserDto?> GetUserById(long telegramId)
        {
            using var context = _contextFactory.CreateDbContext();

            var result = await context.Users
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (result == null)
                return null;

            return new UserDto(result.Id, result.TelegramId, result.CreatedAt);
        }

        public async Task CreateUserAsync(UserDto userDto)
        {
            using var context = _contextFactory.CreateDbContext();

            var existed = await context.Users.SingleOrDefaultAsync(u => u.TelegramId == userDto.TelegramId);

            if (existed == null)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid(),
                    TelegramId = userDto.TelegramId,
                    CreatedAt = DateTime.UtcNow,
                    SubscribtionsUpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasUserWithTelegramIdAsync(long telegramId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Users.AnyAsync(u => u.TelegramId == telegramId);
        }
    }
}
