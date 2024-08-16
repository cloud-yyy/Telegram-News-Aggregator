namespace Shared.Dtos
{
    public record ChannelDto
    {
        public Guid Id { get; init; }
        public long TelegramId { get; init; }
        public string Name { get; init; }

        public ChannelDto()
        {
        }

        public ChannelDto(Guid id, long telegramId, string name)
        {
            Id = id;
            TelegramId = telegramId;
            Name = name;
        }
    }
}