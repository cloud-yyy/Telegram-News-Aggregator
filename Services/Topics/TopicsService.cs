using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace Services.Topics
{
    public class TopicsService
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        public TopicsService(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Task<List<TopicDto>> GetAllTopics(long? userId)
        {
            using var context = _contextFactory.CreateDbContext();

            // TODO: fix this shit
            if (userId != null)
            {
                var user = context.Users
                    .Include(u => u.SubscribedTopics)
                    .AsSplitQuery()
                    .FirstOrDefault(u => u.TelegramId == userId);

                var subscribed = user.SubscribedTopics
                    .Select(t => new TopicDto() { Id = t.Id, Name = t.Name })
                    .ToList();

                return Task.FromResult(subscribed);
            }

            var result = context.Topics
                .Select(t => new TopicDto()
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToList();

            return Task.FromResult(result);
        }

        public async Task<TopicDto> GetTopicById(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();

            var result = await context.Topics
                .FindAsync(id);

            return new TopicDto() { Id = result.Id, Name = result.Name };
        }

        public async Task CreateTopic([FromBody] TopicDto dto)
        {
            using var context = _contextFactory.CreateDbContext();
            
            var topic = new Topic() { Id = Guid.NewGuid(), Name = dto.Name };

            context.Topics.Add(topic);
            await context.SaveChangesAsync();
        }

        public async Task DeleteTopic(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();

            var topic = await context.Topics.FindAsync(id);
            
            if (topic != null)
            {
                context.Topics.Remove(topic);
                await context.SaveChangesAsync();
            }
        }
    }
}