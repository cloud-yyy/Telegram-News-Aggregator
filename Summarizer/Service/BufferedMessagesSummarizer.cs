using AutoMapper;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;
using Summarizer.Contracts;

namespace Summarizer.Service
{
    internal class BufferedMessagesSummarizer
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly IMessagesSummarizer _messagesSummarizer;
        private readonly BufferedBlockService _bufferedBlockService;

        public BufferedMessagesSummarizer(
            IDbContextFactory<ApplicationContext> contextFactory,
            IMapper mapper,
            IMessagesSummarizer messagesSummarizer,
            BufferedBlockService bufferedBlockService)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _messagesSummarizer = messagesSummarizer;
            _bufferedBlockService = bufferedBlockService;
        }

        public async Task<SummaryDto> SummarizeBlockAsync(Guid blockId)
        {
            using var context = _contextFactory.CreateDbContext();

            var messageIds = context.BufferedMessages
                .Where(block => block.BlockId == blockId)
                .Select(b => b.MessageId)
                .ToList();

            var messageDtos = context.Messages
                .Where(m => messageIds.Contains(m.Id))
                .Select(m => _mapper.Map<MessageDto>(m))
                .ToList();

            var summary = await _messagesSummarizer.SummarizeAsync(messageDtos);

            await _bufferedBlockService.DeleteBlock(blockId);

            return summary;
        }
    }
}
