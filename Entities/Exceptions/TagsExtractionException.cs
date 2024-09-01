namespace Entities.Exceptions
{
    public class TagsExtractionException : Exception
    {
        public TagsExtractionException(Exception innerException)
            : base($"Exception occured when extracting tags. Inner exception:\n{innerException.ToString()}")
        {
        }
    }
}