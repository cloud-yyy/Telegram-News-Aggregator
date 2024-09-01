using Entities.Models;
using MessageBroker.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository;
using Shared.Notifications;
using Summarizer.Contracts;

namespace Summarizer.Service
{
    internal class MessageBufferizerService : IMessageBufferizerService, IMessageConsumer<MessageCreatedNotification>
    {
        private readonly MessageComparerBase _messageComparer;
        private readonly ILogger<MessageBufferizerService> _logger;
        private readonly BufferedBlockService _bufferedBlockService;
        private readonly ApplicationContext _context;
        private readonly SemaphoreSlim _semaphore;

        public MessageBufferizerService(
            MessageComparerBase messageComparer,
            BufferedBlockService bufferedBlockService,
            IDbContextFactory<ApplicationContext> contextFactory,
            ILogger<MessageBufferizerService> logger)
        {
            _messageComparer = messageComparer;
            _logger = logger;
            _bufferedBlockService = bufferedBlockService;
            _context = contextFactory.CreateDbContext();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Notify(MessageCreatedNotification message)
        {
            await _semaphore.WaitAsync();

            try
            {
                var similarMessageIds = await _messageComparer.GetSimilarMessagesIdsAsync(message.Id);

                if (!similarMessageIds.Any())
                    return;

                await CreateBlockAsync(message.Id, similarMessageIds);
                await SetMessagesStatusAsync([message.Id, similarMessageIds.First()]);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task CreateBlockAsync(Guid newMessageId, IEnumerable<Guid> similarMessageIds)
        {
            var blockId = await _bufferedBlockService.GetBlockIdForAnyMessageAsync(similarMessageIds);

            if (blockId == null)
            {
                blockId = await _bufferedBlockService.CreateBlockAsync();
                _logger.LogTrace("Buffered block created");
            }

            await _bufferedBlockService.AddMessageToBlockAsync((Guid)blockId, newMessageId);
            await _bufferedBlockService.AddMessageToBlockAsync((Guid)blockId, similarMessageIds.First());

            _logger.LogTrace("Messages added to block");
        }

        private async Task SetMessagesStatusAsync(IEnumerable<Guid> messageIds)
        {
            await _context.Messages
                .Where(m => messageIds.Contains(m.Id))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(m => m.Status, Message.SummarizationStatus.InBlock));

            await _context.SaveChangesAsync();
        }
    }
}
