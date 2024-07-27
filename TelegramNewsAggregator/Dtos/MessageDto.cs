namespace TelegramNewsAggregator
{
    [Serializable]
    public record MessageDto
    {
        public long Id { get; init; }
        public long SenderId { get; init; }
        public string SenderName { get; init; }
        public DateTime SendedAt { get; init; }
        public string Content { get; init; }
        public bool Edited { get; init; } = false;

        public MessageDto(long id, long senderId, string senderName, DateTime sendedAt, string content, bool edited)
        {
            Id = id;
            SenderId = senderId;
            SenderName = senderName;
            SendedAt = sendedAt;
            Content = content;
            Edited = edited;
        }
    }
}
