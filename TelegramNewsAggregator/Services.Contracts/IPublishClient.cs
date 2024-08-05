using Shared.Dtos;

namespace TelegramNewsAggregator
{
    public interface IPublishClient
    {
        public Task Publish(SummarizedMessageDto message);
    }
}
