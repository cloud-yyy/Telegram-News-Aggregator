using Entities.Exceptions;
using Shared.Dtos;

namespace TelegramNewsAggregator
{
    public class MessageBroker
    {
        private readonly Topic<MessageDto> _messagesTopic;
        private readonly Topic<SummarizedMessageDto> _summarizedMessagesTopic;
        private readonly Topic<TagsForMessageDto> _tagsForMessageTopic;

        public MessageBroker(ILogger logger)
        {
            _messagesTopic = new Topic<MessageDto>(logger);
            _summarizedMessagesTopic = new Topic<SummarizedMessageDto>(logger);
            _tagsForMessageTopic = new Topic<TagsForMessageDto>(logger);
        }

        public void Push<T>(T message)
        {
            if (message is MessageDto dto)
                _messagesTopic.Push(dto);
            else if (message is SummarizedMessageDto summarizedDto)
                _summarizedMessagesTopic.Push(summarizedDto);
            else if (message is TagsForMessageDto tagsForMessageDto)
                _tagsForMessageTopic.Push(tagsForMessageDto);
            else
                throw new MessageBrokerTopicNotExistsException(typeof(T));
        }
 
        public void Subscribe<T>(IMessageConsumer<T> reader)
        {
            if (reader is IMessageConsumer<MessageDto> messageReader)
                _messagesTopic.AddConsumer(messageReader);
            else if (reader is IMessageConsumer<SummarizedMessageDto> summarizedMessageReader)
                _summarizedMessagesTopic.AddConsumer(summarizedMessageReader);
            else if (reader is IMessageConsumer<TagsForMessageDto> tagsReader)
                _tagsForMessageTopic.AddConsumer(tagsReader);
            else
                throw new MessageBrokerTopicNotExistsException(typeof(T));
        }

        public void Stop()
        {
            _messagesTopic.Stop();
            _summarizedMessagesTopic.Stop();
        }
    }
}
