namespace Entities.Exceptions
{
    public class SummarizationException : Exception
    {
        public SummarizationException(string innerExceptionMessage)
            : base($"Inner exception: {innerExceptionMessage}")
        {
        }
    }
}