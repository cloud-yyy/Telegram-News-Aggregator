namespace TelegramNewsAggregator
{
    public class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException(string parameterName)
            : base($"Configuration for {parameterName} was not found.")
        {
        }
    }
}
