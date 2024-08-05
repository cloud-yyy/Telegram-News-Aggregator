namespace Shared.Dtos
{
    [Serializable]
    public record MessageDto
    {
        public Guid Id { get; init; }
        public long TelegramId { get; init; }
        public Guid ChannelId { get; init; }
        public DateTime SendedAt { get; init; }
        public string Content { get; init; }

        public MessageDto(Guid id, long telegramId, Guid channelId, DateTime sendedAt, string content)
        {
            Id = id;
            TelegramId = telegramId;
            ChannelId = channelId;
            SendedAt = sendedAt;
            Content = content;
        }
    }
}
