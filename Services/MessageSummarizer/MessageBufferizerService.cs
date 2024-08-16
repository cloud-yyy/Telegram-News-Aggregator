using Entities.Exceptions;
using Microsoft.Extensions.Configuration;
using Services.Contracts;
using Shared.Notifications;

namespace Services
{
    public class MessageBufferizerService : IMessageConsumer<MessageSavedNotification>
    {
        private readonly MessageComparerBase _messageComparer;
        private readonly BufferedBlockService _bufferedBlockService;
        private readonly BufferedMessagesSummarizer _bufferedMessagesSummarizer;
        private readonly MessageStatusDbWriter _messageStatusDbWriter;
        private readonly MessageBroker _broker;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxBufferedBlockSize;

        public MessageBufferizerService(
            MessageComparerBase messageComparer,
            IConfiguration configuration,
            MessageBroker broker,
            BufferedBlockService bufferedBlockService,
            BufferedMessagesSummarizer bufferedMessagesSummarizer,
            MessageStatusDbWriter messageStatusDbWriter)
        {
            _messageComparer = messageComparer;
            _bufferedBlockService = bufferedBlockService;
            _bufferedMessagesSummarizer = bufferedMessagesSummarizer;
            _broker = broker;
            _semaphore = new SemaphoreSlim(1, 1);
            _messageStatusDbWriter = messageStatusDbWriter;
            _maxBufferedBlockSize = configuration.GetValue<int>("MaxBufferedBlockSize");

            if (_maxBufferedBlockSize == 0)
                throw new ConfigurationNotFoundException("MaxBufferedBlockSize");
        }

        public async Task Notify(MessageSavedNotification message)
        {
            await _semaphore.WaitAsync();

            try
            {
                var similarMessageIds = await _messageComparer.GetSimilarMessagesIdsAsync(message.Id);

                if (!similarMessageIds.Any())
                    return;

                var blockId = await CreateBlock(message.Id, similarMessageIds);
                var blockSize = await _bufferedBlockService.GetBlockSize(blockId);

                if (blockSize >= _maxBufferedBlockSize)
                {
                    var summary = await _bufferedMessagesSummarizer.SummarizeBlockAsync(blockId);
                    _broker.Push(summary);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Guid> CreateBlock(Guid newMessageId, IEnumerable<Guid> similarMessageIds)
        {
            var blockId = await _bufferedBlockService.GetBlockIdForAnyMessageAsync(similarMessageIds);

            if (blockId == null)
                blockId = await _bufferedBlockService.CreateBlockAsync();

            await _bufferedBlockService.AddMessageToBlockAsync((Guid)blockId, newMessageId);
            await _bufferedBlockService.AddMessageToBlockAsync((Guid)blockId, similarMessageIds.First());
            await _messageStatusDbWriter.SetInBlockAsync([newMessageId, similarMessageIds.First()]);

            return (Guid)blockId;
        }
    }
}
