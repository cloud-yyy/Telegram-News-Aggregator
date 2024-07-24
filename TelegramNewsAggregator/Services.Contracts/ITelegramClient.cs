namespace TelegramNewsAggregator
{
    public interface ITelegramClient
    {
        public Task<bool> LoginAsync(UserDto user);
        public Task HandleNewMessagesAsync();
    }
}