namespace TelegramNewsAggregator
{
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"INFO:\t {message}.");
        }

        public void LogWarn(string message)
        {
            Console.WriteLine($"WARN:\t {message}.");
        }
        
        public void LogError(string message)
        {
            Console.WriteLine($"ERROR:\t {message}.");
        }
    }
}