using AutoMapper;
using Entities.Models;
using Repository;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class BufferedMessagesSummarizer
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly MessageStatusDbWriter _messageStatusDbWriter;
        private readonly IMessagesSummarizer _messagesSummarizer;
        private readonly BufferedBlockService _bufferedBlockService;

        public BufferedMessagesSummarizer(
            ApplicationContextFactory contextFactory,
            IMapper mapper,
            MessageStatusDbWriter messageStatusDbWriter,
            IMessagesSummarizer messagesSummarizer,
            BufferedBlockService bufferedBlockService)
        {
            _context = contextFactory.Create();
            _mapper = mapper;
            _messageStatusDbWriter = messageStatusDbWriter;
            _messagesSummarizer = messagesSummarizer;
            _bufferedBlockService = bufferedBlockService;
        }

        public async Task<SummaryDto> SummarizeBlockAsync(Guid blockId)
        {
            var messageIds = _context.BufferedMessages
                .Where(block => block.BlockId == blockId)
                .Select(b => b.MessageId)
                .ToList();

            var messageDtos = _context.Messages
                .Where(m => messageIds.Contains(m.Id))
                .Select(m => _mapper.Map<MessageDto>(m))
                .AsEnumerable();

            var summary = await _messagesSummarizer.SummarizeAsync(messageDtos);

            await _bufferedBlockService.DeleteBlock(blockId);
            await SaveSummaryAsync(summary);
            await _messageStatusDbWriter.SetSummarizedMultipleAsync(messageIds);

            return summary;
        }

        private async Task SaveSummaryAsync(SummaryDto dto)
        {
            var summary = _mapper.Map<Summary>(dto);
            _context.Summaries.Add(summary);

            foreach (var source in dto.Sources)
            {
                var summaryBlock = new SummaryBlock()
                {
                    Id = Guid.NewGuid(),
                    SummaryId = summary.Id,
                    MessageId = source.Id
                };

                _context.SummaryBlocks.Add(summaryBlock);
            }

            await _context.SaveChangesAsync();
        }
    }
}
