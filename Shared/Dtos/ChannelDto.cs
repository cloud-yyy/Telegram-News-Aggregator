namespace Shared.Dtos
{
    public record ChannelDto
    {
        public Guid Id { get; init; }
        public long TelegramId { get; init; }
        public string? Tag { get; init; }

        public ChannelDto()
        {
        }

        public ChannelDto(Guid id, long telegramId, string? tag)
        {
            Id = id;
            TelegramId = telegramId;
            Tag = tag;
        }
    }
}