using MessageBroker.Contracts;
using MessageBroker.Service;
using Publisher.Contracts;
using Shared.Dtos;
using Shared.Notifications;
using Summarizer.Contracts;

namespace Aggregator;

public class BrokerConfig
{
    private readonly Broker _broker;
    private readonly IPublishClient _publishClient;
    private readonly IMessageBufferizerService _bufferizerService;

    public BrokerConfig(
		Broker broker,
		IPublishClient publishClient,
		IMessageBufferizerService bufferizerService)
	{
		_broker = broker;
		_publishClient = publishClient;
		_bufferizerService = bufferizerService;
	}

	public void Configure()
	{
		var bufferedBlockCreatorConsumer = _bufferizerService as IMessageConsumer<MessageCreatedNotification>;
		var publishClientConsumer = _publishClient as IMessageConsumer<SummaryDto>;

		_broker.Subscribe(bufferedBlockCreatorConsumer!);
		_broker.Subscribe(publishClientConsumer!);
	}
}
