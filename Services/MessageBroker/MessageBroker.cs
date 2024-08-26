using Entities.Exceptions;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using Shared.Notifications;

namespace Services
{
    public class MessageBroker
    {
        private readonly Topic<MessageDto> _messagesTopic;
        private readonly Topic<SummaryDto> _summarizedMessagesTopic;
        private readonly Topic<MessageSavedNotification> _messageSavedNotificationTopic;

        public MessageBroker(ILogger<MessageBroker> logger)
        {
            _messagesTopic = new Topic<MessageDto>(logger);
            _summarizedMessagesTopic = new Topic<SummaryDto>(logger);
            _messageSavedNotificationTopic = new Topic<MessageSavedNotification>(logger);
        }

        public void Push<T>(T message)
        {
            if (message is MessageDto dto)
                _messagesTopic.Push(dto);
            else if (message is SummaryDto summarizedDto)
                _summarizedMessagesTopic.Push(summarizedDto);
            else if (message is MessageSavedNotification messageSavedNotification)
                _messageSavedNotificationTopic.Push(messageSavedNotification);
            else
                throw new MessageBrokerTopicNotExistsException(typeof(T));
        }
 
        public void Subscribe<T>(IMessageConsumer<T> reader)
        {
            if (reader is IMessageConsumer<MessageDto> messageReader)
                _messagesTopic.AddConsumer(messageReader);
            else if (reader is IMessageConsumer<SummaryDto> summarizedMessageReader)
                _summarizedMessagesTopic.AddConsumer(summarizedMessageReader);
            else if (reader is IMessageConsumer<MessageSavedNotification> messageSavedReader)
                _messageSavedNotificationTopic.AddConsumer(messageSavedReader);
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
