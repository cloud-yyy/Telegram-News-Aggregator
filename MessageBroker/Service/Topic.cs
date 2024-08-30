using System.Collections.Concurrent;
using MessageBroker.Contracts;
using Microsoft.Extensions.Logging;

namespace MessageBroker.Service;

internal class Topic<T>
{
    private readonly BlockingCollection<T> _queue;
    private readonly Task _executionTask;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly List<IMessageConsumer<T>> _consumers;
    private readonly ILogger _logger;

    public Topic(ILogger<Broker> logger)
    {
        _queue = new();
        _consumers = new();
        _cancellationTokenSource = new();
        _executionTask = Task.Factory.StartNew
            (Process, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        _logger = logger;
    }

    public void Push(T message)
    {
        _queue.Add(message);
        _logger.LogTrace($"Message pushed to {typeof(T)} topic in thread {Environment.CurrentManagedThreadId}");
    }

    public void AddConsumer(IMessageConsumer<T> consumer)
    {
        _consumers.Add(consumer);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _queue.CompleteAdding();
        _executionTask.Wait();
    }

    private void Process()
    {
        foreach (var message in _queue.GetConsumingEnumerable(_cancellationTokenSource.Token))
        {
            foreach (var consumer in _consumers)
            {
                _logger.LogTrace($"Consumer notified: {consumer.GetType().Name} in thread {Environment.CurrentManagedThreadId}");
                consumer.Notify(message);
            }
        }
    }
}
