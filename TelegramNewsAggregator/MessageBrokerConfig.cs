using Services;
using Services.Contracts;
using Shared.Dtos;

namespace TelegramNewsAggregator;

public class MessageBrokerConfig
{
    private readonly MessageBroker _broker;
    private readonly IPublishClient _publishClient;
    private readonly MessageWithTagsSaver _messageWithTagsSaver;

    public MessageBrokerConfig(
		MessageBroker broker,
		MessageWithTagsSaver messageWithTagsSaver,
		IPublishClient publishClient,
		SummarizingEntryPoint summarizingEntryPoint)
	{
		_broker = broker;
		_publishClient = publishClient;
		_messageWithTagsSaver = messageWithTagsSaver;
	}

	public void Configure()
	{
		var messageWithTagsSaverConsumer = _messageWithTagsSaver as IMessageConsumer<MessageDto>;
		var publishClientConsumer = _publishClient as IMessageConsumer<SummaryDto>;

		_broker.Subscribe(messageWithTagsSaverConsumer);
		_broker.Subscribe(publishClientConsumer!);
	}
}
