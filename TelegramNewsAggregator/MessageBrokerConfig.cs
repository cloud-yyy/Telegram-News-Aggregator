using Services;
using Services.Contracts;
using Shared.Dtos;
using Shared.Notifications;

namespace TelegramNewsAggregator;

public class MessageBrokerConfig
{
    private readonly MessageBroker _broker;
    private readonly IPublishClient _publishClient;
    private readonly MessageWithTagsSaver _messageWithTagsSaver;
    private readonly MessageBufferizerService _bufferedBlockCreator;

    public MessageBrokerConfig(
		MessageBroker broker,
		MessageWithTagsSaver messageWithTagsSaver,
		IPublishClient publishClient,
		MessageBufferizerService bufferedBlocksCreator)
	{
		_broker = broker;
		_publishClient = publishClient;
		_messageWithTagsSaver = messageWithTagsSaver;
		_bufferedBlockCreator = bufferedBlocksCreator;
	}

	public void Configure()
	{
		var messageWithTagsSaverConsumer = _messageWithTagsSaver as IMessageConsumer<MessageDto>;
		var bufferedBlockCreatorConsumer = _bufferedBlockCreator as IMessageConsumer<MessageSavedNotification>;
		var publishClientConsumer = _publishClient as IMessageConsumer<SummaryDto>;

		_broker.Subscribe(messageWithTagsSaverConsumer);
		_broker.Subscribe(bufferedBlockCreatorConsumer!);
		_broker.Subscribe(publishClientConsumer!);
	}
}
