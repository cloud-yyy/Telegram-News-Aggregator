using Entities.Models;

namespace Summarizer.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<Message> FilterMessagesToSummarize
            (this IQueryable<Message> queryable, TimeSpan messageLifetime)
        {
            return queryable
                .Where(m => m.Status == Message.SummarizationStatus.NotSummarized
                    && DateTime.UtcNow - m.SendedAt >= messageLifetime);
        }

        public static IQueryable<BufferedBlock> FilterBlocksToSummarize
            (this IQueryable<BufferedBlock> queryable, TimeSpan blockLifetime, int maxSize)
        {
            return queryable
                .Where(b => DateTime.UtcNow - b.UpdatedAt >= blockLifetime
                    || b.Size >= maxSize);
        }
    }
}
