namespace TelegramNewsAggregator
{
    public interface IMessageSerializer
    {
        public void Serialize(MessageDto message);
    }
}