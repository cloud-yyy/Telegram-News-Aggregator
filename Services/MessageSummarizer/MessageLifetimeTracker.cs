using AutoMapper;
using Entities.Models;
using Microsoft.Extensions.Configuration;
using Repository;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class MessageLifetimeTracker
    {
        private readonly ApplicationContext _context;
        private readonly IMessagesSummarizer _messageSummarizer;
        private readonly BufferedMessagesSummarizer _bufferedMessagesSummarizer;
        private readonly BufferedBlockService _bufferedBlockService;
        private readonly IMapper _mapper;
        private readonly MessageBroker _broker;
        private readonly MessageStatusDbWriter _messageStatusDbWriter;

        private readonly TimeSpan _messageLifetime = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _blockLifetime = TimeSpan.FromMinutes(1);
        private readonly int _delayInSeconds = 1;

        private readonly Task _executionTask;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public MessageLifetimeTracker(
            ApplicationContextFactory contextFactory, 
            IMessagesSummarizer messagesSummarizer,
            BufferedMessagesSummarizer bufferedMessagesSummarizer,
            BufferedBlockService bufferedBlockService,
            IMapper mapper,
            MessageBroker broker,
            MessageStatusDbWriter messageStatusDbWriter,
            IConfiguration configuration)
        {
            _context = contextFactory.Create();
            _messageSummarizer = messagesSummarizer;
            _bufferedMessagesSummarizer = bufferedMessagesSummarizer;
            _bufferedBlockService = bufferedBlockService;
            _mapper = mapper;
            _broker = broker;
            _messageStatusDbWriter = messageStatusDbWriter;

            _messageLifetime = TimeSpan.FromSeconds(configuration.GetValue<int>("MessageLifetimeInSeconds"));
            _blockLifetime = TimeSpan.FromSeconds(configuration.GetValue<int>("BlockLifetimeInSeconds"));
            _delayInSeconds = configuration.GetValue<int>("LifetimeCheckDelayInSeconds");

            _cancellationTokenSource = new();
            _executionTask = Task.Factory.StartNew
                (ExecuteAsync, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task ExecuteAsync()
        {
            while (true)
            {
                var messages = GetNotSummarizedSingleMessages();

                if (messages.Any())
                    await SummarizeSingleMessages(messages);

                var blocks = GetNotSummarizedBlocks();

                if (blocks.Any())
                    await SummarizeBlocks(blocks);

                await Task.Delay(_delayInSeconds * 1000);
            }
        }

        private IEnumerable<MessageDto> GetNotSummarizedSingleMessages()
        {
            var messages = _context.Messages
                .Where(m => m.Status == Message.SummarizationStatus.NotSummarized
                    && DateTime.UtcNow - m.SendedAt >= _messageLifetime)
                .Select(m => _mapper.Map<MessageDto>(m))
                .ToList();

            return messages;
        }

        private async Task SummarizeSingleMessages(IEnumerable<MessageDto> messages)
        {
            foreach (var message in messages)
            {
                var summary = await _messageSummarizer.SummarizeAsync([message]);
                _broker.Push(summary);
                await SaveSummaryAsync(summary);
            }

            var ids = messages.Select(m => m.Id);
            await _messageStatusDbWriter.SetSummarizedSingleAsync(ids);
        }

        private IEnumerable<BufferedMessageDto> GetNotSummarizedBlocks()
        {
            var blockIds = _context.BufferedBlocks
                .Where(b => DateTime.UtcNow - b.UpdatedAt >= _blockLifetime)
                .Select(b => b.Id)
                .AsEnumerable();

            var messages = _context.BufferedMessages
                .Where(m => blockIds.Contains(m.BlockId))
                .Select(m => _mapper.Map<BufferedMessageDto>(m))
                .AsEnumerable();

            return messages;
        }

        private async Task SummarizeBlocks(IEnumerable<BufferedMessageDto> messages)
        {
            foreach (var id in messages.Select(m => m.BlockId))
            {
                var summary = await _bufferedMessagesSummarizer.SummarizeBlockAsync(id);
                _broker.Push(summary);
                await _bufferedBlockService.DeleteBlock(id);
            }

            await _messageStatusDbWriter.SetSummarizedMultipleAsync(messages.Select(b => b.MessageId));
        }

        private async Task SaveSummaryAsync(SummaryDto dto)
        {
            var summary = _mapper.Map<Summary>(dto);
            _context.Summaries.Add(summary);

            await _context.SaveChangesAsync();
            
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
