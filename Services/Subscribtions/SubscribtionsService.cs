using Microsoft.EntityFrameworkCore;
using Repository;

namespace Services.Subscribtions
{
    public class SubscribtionsService
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;

        public SubscribtionsService(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task SubscribeOnTopic(long userId, Guid topicId)
        {
            using var context = _contextFactory.CreateDbContext();

            // user including his subscribed topics
            var user = await context.Users
                .Include(u => u.SubscribedTopics)
                .AsSplitQuery()
                .SingleOrDefaultAsync(u => u.TelegramId == userId);

            // subscribing
            if (!user.SubscribedTopics.Any(c => c.Id == topicId))
            {
                var topic = await context.Topics.FindAsync(topicId);
                user.SubscribedTopics.Add(topic);
                await context.SaveChangesAsync();
            }
        }

        public async Task UnsubscribeOnTopic(long userId, Guid topicId)
        {
            using var context = _contextFactory.CreateDbContext();
            
            // user including his subscribed topics
            var user = await context.Users
                .Include(u => u.SubscribedTopics)
                .AsSplitQuery()
                .SingleOrDefaultAsync(u => u.TelegramId == userId);

            var topic = user.SubscribedTopics.FirstOrDefault(t => t.Id == topicId);

            if (topic != null)
            {
                user.SubscribedTopics.Remove(topic);
                await context.SaveChangesAsync();
            }
        }
    }
}
