using Shared.Dtos;

namespace Publisher.Contracts;

public interface IPublishClient
{
    public Task Publish(SummaryDto message);
}
