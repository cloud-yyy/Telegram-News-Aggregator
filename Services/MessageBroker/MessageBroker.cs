using Entities.Exceptions;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class MessageBroker
    {
        private readonly Topic<MessageDto> _messagesTopic;
        private readonly Topic<SummaryDto> _summarizedMessagesTopic;
        private readonly Topic<MessageTagsDto> _tagsForMessageTopic;

        public MessageBroker(ILogger logger)
        {
            _messagesTopic = new Topic<MessageDto>(logger);
            _summarizedMessagesTopic = new Topic<SummaryDto>(logger);
            _tagsForMessageTopic = new Topic<MessageTagsDto>(logger);
        }

        public void Push<T>(T message)
        {
            if (message is MessageDto dto)
                _messagesTopic.Push(dto);
            else if (message is SummaryDto summarizedDto)
                _summarizedMessagesTopic.Push(summarizedDto);
            else if (message is MessageTagsDto tagsForMessageDto)
                _tagsForMessageTopic.Push(tagsForMessageDto);
            else
                throw new MessageBrokerTopicNotExistsException(typeof(T));
        }
 
        public void Subscribe<T>(IMessageConsumer<T> reader)
        {
            if (reader is IMessageConsumer<MessageDto> messageReader)
                _messagesTopic.AddConsumer(messageReader);
            else if (reader is IMessageConsumer<SummaryDto> summarizedMessageReader)
                _summarizedMessagesTopic.AddConsumer(summarizedMessageReader);
            else if (reader is IMessageConsumer<MessageTagsDto> tagsReader)
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
