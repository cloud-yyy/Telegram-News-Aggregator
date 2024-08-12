using AutoMapper;
using Microsoft.Extensions.Configuration;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class SummarizingEntryPoint
    {
        private readonly Task _workingTask;
        private readonly MessagesTagsDbReader _messageTagsDbReader;
        private readonly IMessageComparer _messagesComparer;
        private readonly IMessagesSummarizer _messageSummarizer;
        private readonly IMapper _mapper;
        private readonly MessageBroker _broker;
        private readonly MessageStatusDbWriter _messageStatusDbWriter;
        private readonly MessageDbReader _messageDbReader;
        private readonly SummaryDbWriter _summaryDbWriter;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private int _delayInSeconds;

        public SummarizingEntryPoint(
            MessagesTagsDbReader messagesTagsDbReader,
            IMessageComparer messagesComparer,
            IMessagesSummarizer messageSummarizer,
            IMapper mapper,
            MessageBroker broker,
            MessageStatusDbWriter messageStatusDbWriter,
            MessageDbReader messageDbReader,
            SummaryDbWriter summaryDbWriter,
            ILogger logger,
            IConfiguration configuration)
        {
            _messageTagsDbReader = messagesTagsDbReader;
            _messagesComparer = messagesComparer;
            _messageSummarizer = messageSummarizer;
            _mapper = mapper;
            _broker = broker;
            _messageStatusDbWriter = messageStatusDbWriter;
            _messageDbReader = messageDbReader;
            _summaryDbWriter = summaryDbWriter;
            _logger = logger;
            
            _cancellationTokenSource = new();
            _workingTask = Task.Factory.StartNew
                (Process, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning,  TaskScheduler.Default);

            _delayInSeconds = configuration.GetValue<int>("SummarizationDelay");
        }

        public async Task Process()
        {
            while (true)
            {
                _logger.LogInfo($"Summarizing thread is sleeping for {_delayInSeconds} seconds...");
                await Task.Delay(_delayInSeconds * 1000);
                _logger.LogInfo("Summarizing thread awake!");
                
                await SummarizeBlocksAsync();
            }
        }

        public void StopProcessing()
        {
            _cancellationTokenSource.Cancel();
            _workingTask.Wait();
        }

        private async Task SummarizeBlocksAsync()
        {
            var messagesWithTags = _messageTagsDbReader
                .ReadNotSummmarizedMessagesJoinedTags();

            if (!messagesWithTags.Any())
                return;

            var comparingResult = await _messagesComparer.CompareByTags(messagesWithTags);

            foreach (var block in comparingResult.Results)
            {
                var sources = await FindSourcesByIdsAsync(block.MessagesIds);
                var summarized = await _messageSummarizer.Summarize(sources);
                _broker.Push(summarized);
                await _summaryDbWriter.SaveSummaryAsync(summarized);
            }

            await UpdateMessagesStatus(comparingResult);
        }

        private async Task<List<MessageDto>> FindSourcesByIdsAsync(IEnumerable<Guid> ids)
        {
            var sources = new List<MessageDto>();

            foreach (var id in ids)
            {
                var message = await _messageDbReader.GetMessageAsync(id);
                var dto = _mapper.Map<MessageDto>(message);
                sources.Add(dto!);
            }

            return sources;
        }

        private async Task UpdateMessagesStatus(MessagesComparingResultDto dto)
        {
            var idsToUpdateMultiple = dto.Results
                .Where(b => b.MessagesIds.Count > 1)
                .SelectMany(b => b.MessagesIds);

            var idsToUpdateSingle = dto.Results
                .Where(b => b.MessagesIds.Count == 1)
                .SelectMany(b => b.MessagesIds);

            await _messageStatusDbWriter.SetSummarizedMultipleAsync(idsToUpdateMultiple);
            await _messageStatusDbWriter.SetSummarizedSingleAsync(idsToUpdateSingle);
        }
    }
}
