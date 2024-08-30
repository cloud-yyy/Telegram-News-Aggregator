using Entities.Models;
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
            var newMessage = await _context.Messages.FindAsync(newMessageId);

            var tags = ReadNotSummmarizedMessagesJoinedTags();
            var newMessageTags = tags.SingleOrDefault(mt => mt.MessageId == newMessageId);
            var existedMessagesTags = tags.Where(mt => mt.MessageId != newMessageId);

            if (!existedMessagesTags.Any())
                return [];

            return await CompareToMessageAsync(newMessageTags, existedMessagesTags);
        }

        private IEnumerable<MessageTagsDto> ReadNotSummmarizedMessagesJoinedTags()
        {
            var notSummarizedMessagesIds = _context.Messages
                .Where(m => m.Status == Message.SummarizationStatus.NotSummarized
                    || m.Status == Message.SummarizationStatus.InBlock)
                .Select(m => m.Id);

            return JoinMessageTagsToTags(notSummarizedMessagesIds);
        }

        private IEnumerable<MessageTagsDto> JoinMessageTagsToTags(IEnumerable<Guid> filterIds)
        {
            var messagesJoinedTags = _context.MessagesTags
                .Where(mt => filterIds.Contains(mt.MessageId))
                .Join(
                    _context.Tags,
                    mt => mt.TagId,
                    t => t.Id,
                    (mt, t) => new { MessageId = mt.MessageId, Tag = t.Name }
                );

            var messagesWithTagsMap = new Dictionary<Guid, List<string>>();

            foreach (var item in messagesJoinedTags)
            {
                if (!messagesWithTagsMap.ContainsKey(item.MessageId))
                    messagesWithTagsMap.Add(item.MessageId, []);

                messagesWithTagsMap[item.MessageId].Add(item.Tag);
            }

            return messagesWithTagsMap.Select(keyPair => new MessageTagsDto(keyPair.Key, keyPair.Value));
        }

        protected abstract Task<IEnumerable<Guid>> CompareToMessageAsync
            (MessageTagsDto newMessageTags, IEnumerable<MessageTagsDto> existedMessagesTags);
    }
}
