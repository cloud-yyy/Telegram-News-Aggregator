using Shared.Dtos;

namespace Services.Contracts
{
    public interface IPublishClient
    {
        public Task Publish(SummaryDto message);
    }
}
