namespace Entities.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(long telegramId)
            : base($"User with telegramId {telegramId} was not found")
        {
        }
    }
}
