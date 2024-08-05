namespace Entities.Exceptions
{
    public class MessageBrokerTopicNotExistsException : Exception
    {
        public MessageBrokerTopicNotExistsException(Type messageType)
            : base($"Message Broker topic for messsages of type {messageType} does not exist.")
        {
        }
    }
}