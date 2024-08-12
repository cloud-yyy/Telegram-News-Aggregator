using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace Services
{
    public class MessagesTagsDbReader
    {
        private readonly RepositoryContext _context;

        public MessagesTagsDbReader(RepositoryContextFactory contextFactory)
        {
            _context = contextFactory.Create();
        }

        public IEnumerable<MessageTagsDto> ReadNotSummmarizedMessagesJoinedTags()
        {
            var notSummarizedMessagesIds = _context.Messages
                .Where(m => m.Status == Message.SummarizationStatus.NotSummarized)
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
    }
}
