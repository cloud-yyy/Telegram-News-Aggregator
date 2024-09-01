namespace Entities.Exceptions
{
    public class MessagesComparingException : Exception
    {
        public MessagesComparingException(string innerExeption)
            : base($"Error while compare messages. Inner exception: {innerExeption}")
        {
        }
    }
}