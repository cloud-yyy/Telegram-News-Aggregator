
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace TelegramNewsAggregator
{
    public class MessageSerializer : IMessageSerializer
    {
        private readonly BlockingCollection<SummarizedMessageDto> _messageQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _processingTask;
        private readonly string _destinationFile;

        public MessageSerializer(IConfiguration configuration)
        {
            _destinationFile = configuration.GetValue<string>("MessagesDestinationFile");
            _messageQueue = new();
            _cancellationTokenSource = new();

            _processingTask = Task.Factory.StartNew
                (ProcessQueue, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void SerializeMessage(SummarizedMessageDto message)
        {
            _messageQueue.Add(message);
        }

        private void ProcessQueue()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
            {
                var json = JsonConvert.SerializeObject(message);
                File.AppendAllText(_destinationFile, $"{json},\n");
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();
            _processingTask.Wait();
        }
    }
}
