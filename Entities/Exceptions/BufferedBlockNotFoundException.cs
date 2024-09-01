namespace Entities.Exceptions
{
    public class BufferedBlockNotFoundException : Exception
    {
        public BufferedBlockNotFoundException(Guid id)
            : base($"Block with id {id} not found")
        {
        }
    }
}