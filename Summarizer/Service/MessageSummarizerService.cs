using AutoMapper;
using Entities.Models;
using MessageBroker.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repository;
using Shared.Dtos;
using Shared.Params;
using Summarizer.Contracts;

namespace Summarizer.Service
{
    internal class MessageSummarizerService : IMessageSummarizerService
    {
        private readonly ApplicationContext _context;
        private readonly IMessagesSummarizer _messageSummarizer;
        private readonly BufferedMessagesSummarizer _bufferedMessagesSummarizer;
        private readonly BufferedBlockService _bufferedBlockService;
        private readonly IMapper _mapper;
        private readonly ILogger<MessageSummarizerService> _logger;
        private readonly Broker _broker;

        private readonly BufferizationParams _bufferizationParams;

        private Task? _executionTask;
        private CancellationTokenSource? _cancellationTokenSource;

        public MessageSummarizerService(
            IDbContextFactory<ApplicationContext> contextFactory,
            ILogger<MessageSummarizerService> logger,
            IMessagesSummarizer messagesSummarizer,
            BufferedMessagesSummarizer bufferedMessagesSummarizer,
            BufferedBlockService bufferedBlockService,
            IMapper mapper,
            Broker broker,
            IConfiguration configuration)
        {
            _context = contextFactory.CreateDbContext();
            _messageSummarizer = messagesSummarizer;
            _bufferedMessagesSummarizer = bufferedMessagesSummarizer;
            _bufferedBlockService = bufferedBlockService;
            _mapper = mapper;
            _logger = logger;
            _broker = broker;

            var config = configuration.GetSection("BufferizationParams");

            _bufferizationParams = new
            (
                maxBufferedBlockSize: int.Parse(config["MaxBufferedBlockSize"]!),
                messageLifetimeInSeconds: TimeSpan.FromSeconds(int.Parse(config["MessageLifetimeInSeconds"]!)),
                blockLifetimeInSeconds: TimeSpan.FromSeconds(int.Parse(config["BlockLifetimeInSeconds"]!)),
                lifetimeCheckDelayInSeconds: int.Parse(config["LifetimeCheckDelayInSeconds"]!)
            );
        }
        
        public Task StartAsync()
        {
            _cancellationTokenSource = new();

            _executionTask = Task.Factory.StartNew
                (Process, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return Task.CompletedTask;
        }

        private async Task Process()
        {
            while (true)
            {
                _logger.LogTrace("Checking block and messages lifetime started");

                var messages = GetMessagesForSummarization();

                if (messages.Any())
                {
                    var ids = await SummarizeSingleMessages(messages);
                    await UpdateMessagesStatusAsync(ids, Message.SummarizationStatus.SummarizedSingle);
                }

                var blockIds = GetBlocksForSummarization();

                if (blockIds.Any())
                {
                    var ids = await SummarizeBlocks(blockIds);
                    await UpdateMessagesStatusAsync(ids, Message.SummarizationStatus.SummarizedMultiple);
                }

                await _context.SaveChangesAsync();

                _logger.LogTrace("Checking block and messages lifetime ended");
                
                await Task.Delay(_bufferizationParams.LifetimeCheckDelayInSeconds * 1000);
            }
        }

        private IEnumerable<MessageDto> GetMessagesForSummarization()
        {
            var messages = _context.Messages
                .Where(m => m.Status == Message.SummarizationStatus.NotSummarized
                    && DateTime.UtcNow - m.SendedAt >= _bufferizationParams.MessageLifetimeInSeconds)
                .Select(m => _mapper.Map<MessageDto>(m))
                .ToList();

            return messages;
        }

        private async Task<IEnumerable<Guid>> SummarizeSingleMessages(IEnumerable<MessageDto> messages)
        {
            foreach (var message in messages)
            {
                var summary = await _messageSummarizer.SummarizeAsync([message]);
                _broker.Push(summary);
                await SaveSummaryAsync(summary);
            }

            return messages.Select(m => m.Id);
        }

        private IEnumerable<Guid> GetBlocksForSummarization()
        {
            var ids = _context.BufferedBlocks
                .Where(b => DateTime.UtcNow - b.UpdatedAt >= _bufferizationParams.BlockLifetimeInSeconds
                    || b.Size >= _bufferizationParams.MaxBlockSize)
                .Select(b => b.Id)
                .AsEnumerable();

            return ids;
        }

        private async Task<IEnumerable<Guid>> SummarizeBlocks(IEnumerable<Guid> blockIds)
        {
            foreach (var id in blockIds)
            {
                var summary = await _bufferedMessagesSummarizer.SummarizeBlockAsync(id);
                _broker.Push(summary);
                await _bufferedBlockService.DeleteBlock(id);
            }

            return _context.BufferedMessages
                .Where(m => blockIds.Contains(m.BlockId))
                .Select(m => m.Id)
                .AsEnumerable();
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

        private async Task UpdateMessagesStatusAsync(IEnumerable<Guid> ids, Message.SummarizationStatus status)
        {
            await _context.Messages
                .Where(m => ids.Contains(m.Id))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(m => m.Status, status));
        }
    }
}
