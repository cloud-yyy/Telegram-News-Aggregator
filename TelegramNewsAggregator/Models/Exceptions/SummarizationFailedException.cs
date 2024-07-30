namespace TelegramNewsAggregator
{
    public class SummarizationException : Exception
    {
        public SummarizationException(string innerExceptionMessage)
            : base($"Inner exception: {innerExceptionMessage}")
        {
        }
    }
}