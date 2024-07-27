namespace TelegramNewsAggregator
{
    public interface IMessageSerializer
    {
        public void SerializeMessage(SummarizedMessageDto message);
    }
}
