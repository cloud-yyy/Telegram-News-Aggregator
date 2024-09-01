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
using Summarizer.Extensions;

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

        public void Start()
        {
            _cancellationTokenSource = new();

            _executionTask = Task.Factory.StartNew
                (Process, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task Process()
        {
            while (true)
            {
                _logger.LogTrace("Checking block and messages lifetime started");

                await TrySummarizeMessages();
                await TrySummarizeBlocks();

                _logger.LogTrace("Checking block and messages lifetime ended");

                await Task.Delay(_bufferizationParams.LifetimeCheckDelayInSeconds * 1000);
            }
        }

        private async Task TrySummarizeMessages()
        {
            var messages = _context.Messages
                .FilterMessagesToSummarize(_bufferizationParams.MessageLifetime)
                .Select(m => _mapper.Map<MessageDto>(m))
                .ToList();

            if (messages.Count != 0)
            {
                var ids = await SummarizeMessages(messages);
                await _context.SaveChangesAsync();
                await UpdateMessagesStatusAsync(ids, Message.SummarizationStatus.SummarizedSingle);
            }
        }

        private async Task TrySummarizeBlocks()
        {
            var ids = _context.BufferedBlocks
                .FilterBlocksToSummarize(_bufferizationParams.BlockLifetime, _bufferizationParams.MaxBlockSize)
                .Select(b => b.Id)
                .ToList();

            if (ids.Count != 0)
            {
                var summarizedBlocksIds = await SummarizeBlocks(ids);
                await _context.SaveChangesAsync();
                await UpdateMessagesStatusAsync(summarizedBlocksIds, Message.SummarizationStatus.SummarizedMultiple);
            }
        }

        private async Task<IEnumerable<Guid>> SummarizeMessages(IEnumerable<MessageDto> messages)
        {
            foreach (var message in messages)
            {
                var summary = await _messageSummarizer.SummarizeAsync([message]);
                _broker.Push(summary);
                CreateSummaryAsync(summary);
            }

            return messages.Select(m => m.Id);
        }

        private async Task<IEnumerable<Guid>> SummarizeBlocks(IEnumerable<Guid> blockIds)
        {
            var ids = new List<MessageDto>();

            foreach (var id in blockIds)
            {
                var summary = await _bufferedMessagesSummarizer.SummarizeBlockAsync(id);
                _broker.Push(summary);

                ids.AddRange(summary.Sources);

                await _bufferedBlockService.DeleteBlock(id);
                CreateSummaryAsync(summary);
            }

            return ids
                .Select(m => m.Id)
                .AsEnumerable();
        }

        private void CreateSummaryAsync(SummaryDto dto)
        {
            var summary = _mapper.Map<Summary>(dto);
            summary.Sources.Clear();

            foreach (var source in dto.Sources)
            {
                var message = _context.Messages.Find(source.Id);
                summary.Sources.Add(message!);
            }

            _context.Summaries.Add(summary);
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
