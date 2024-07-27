namespace TelegramNewsAggregator
{
    public interface IMessageConsumer<T>
    {
        public Task Notify(T message);
    }
}
