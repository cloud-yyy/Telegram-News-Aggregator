using Shared.Dtos;

namespace TelegramNewsAggregator
{
    public class SummarizeServiceMock : ISummarizeService, IMessageConsumer<MessageDto>
    {
        private readonly MessageBroker _broker;
        private readonly ILogger _logger;

        public SummarizeServiceMock(MessageBroker broker, ILogger logger)
        {
            _broker = broker;
            _logger = logger;
        }

        public Task Notify(MessageDto message)
        {
            _logger.LogInfo($"SummarizeServiceMock notified in thread: {Environment.CurrentManagedThreadId}");
            return Summarize(message);
        }

        public Task Summarize(MessageDto message)
        {
            _logger.LogInfo($"Summarization started in thread: {Environment.CurrentManagedThreadId}");
            var summarized = new SummarizedMessageDto(message.Id, message.TelegramId, "Mocked title", "Mocked summary", "t.me/");
            _logger.LogInfo($"Summarization ended in thread: {Environment.CurrentManagedThreadId}");
            _broker.Push(summarized);
            return Task.CompletedTask;
        }
    }
}
