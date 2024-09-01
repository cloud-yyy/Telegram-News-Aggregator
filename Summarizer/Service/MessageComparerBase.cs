using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace Summarizer.Service
{
    internal abstract class MessageComparerBase
    {
        private readonly ApplicationContext _context;

        public MessageComparerBase(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _context = contextFactory.CreateDbContext();
        }

        public async Task<IEnumerable<Guid>> GetSimilarMessagesIdsAsync(Guid newMessageId)
        {
            var messageTagsDtos = _context.Messages
                .Include(m => m.Tags)
                .Select(m => new MessageTagsDto(m.Id, m.Tags.Select(t => t.Name)))
                .AsEnumerable();

            var newTags = messageTagsDtos.SingleOrDefault(t => t.MessageId == newMessageId);
            var existedTags = messageTagsDtos.Where(t => t.MessageId != newMessageId);

            if (!existedTags.Any())
                return [];

            return await CompareToMessageAsync(newTags!, existedTags);
        }

        protected abstract Task<IEnumerable<Guid>> CompareToMessageAsync
            (MessageTagsDto newMessageTags, IEnumerable<MessageTagsDto> existedMessagesTags);
    }
}
