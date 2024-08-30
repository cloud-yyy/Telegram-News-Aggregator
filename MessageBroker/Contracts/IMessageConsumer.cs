namespace MessageBroker.Contracts;

public interface IMessageConsumer<T>
{
    public Task Notify(T message);
}
