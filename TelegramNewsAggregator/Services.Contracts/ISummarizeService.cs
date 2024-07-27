
namespace TelegramNewsAggregator
{
    public interface ISummarizeService
    {
        public Task Summarize(MessageDto message);
    }
}