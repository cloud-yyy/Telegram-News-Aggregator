using Shared.Dtos;
using TelegramNewsAggregator.Services;

namespace TelegramNewsAggregator;

public class MessageBrokerConfig
{
    private readonly MessageBroker _broker;
    private readonly ISummarizeService _summarizeService;
    private readonly IPublishClient _publishClient;
	private readonly MessageDbWriter _messageDbWriter;
    private readonly ITagsExtractService _tagsExtractService;
    private readonly TagsDbWriter _tagsDbWriter;

    public MessageBrokerConfig(
		MessageBroker broker, 
		ISummarizeService summarizeService, 
		IPublishClient publishClient,
		MessageDbWriter messageDbWriter,
		ITagsExtractService tagsExtractService,
		TagsDbWriter tagsDbWriter)
	{
		_broker = broker;
		_summarizeService = summarizeService;
		_publishClient = publishClient;
		_messageDbWriter = messageDbWriter;
		_tagsExtractService = tagsExtractService;
		_tagsDbWriter = tagsDbWriter;
	}

	public void Configure()
	{
		var summarizeServiceConsumer = _summarizeService as IMessageConsumer<MessageDto>;
		var messageDbWriterConsumer = _messageDbWriter as IMessageConsumer<MessageDto>;
		var tagsExtractServiceConsumer = _tagsExtractService as IMessageConsumer<MessageDto>;
		var publishClientConsumer = _publishClient as IMessageConsumer<SummarizedMessageDto>;
		var tagsDbWriterConsumer = _tagsDbWriter as IMessageConsumer<TagsForMessageDto>;

		_broker.Subscribe(summarizeServiceConsumer!);
		_broker.Subscribe(tagsExtractServiceConsumer!);
		_broker.Subscribe(messageDbWriterConsumer!);
		_broker.Subscribe(publishClientConsumer!);
		_broker.Subscribe(tagsDbWriterConsumer!);
	}
}
