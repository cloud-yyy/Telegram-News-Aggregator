namespace Entities.Exceptions
{
    public class ChannelNotFoundException : Exception
    {
        public ChannelNotFoundException(Guid id)
            : base($"Channel with id {id} not found.")
        {
        }

        public ChannelNotFoundException(long telegramId)
            : base($"Channel with Telegram ID {telegramId} not found.")
        {
        }

        public ChannelNotFoundException(string tag)
            : base($"Channel with tag {tag} not found.")
        {
        }
    }
}