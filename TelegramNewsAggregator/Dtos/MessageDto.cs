namespace TelegramNewsAggregator
{
    [Serializable]
    public record MessageDto
    {
        public long SenderId { get; init; }
        public string SenderName { get; init; }
        public DateTime SendedAt { get; init; }
        public string Content { get; init; }
        public bool Edited { get; init; } = false;

        public MessageDto(long senderId, string senderName, DateTime sendedAt, string content, bool edited)
        {
            SenderId = senderId;
            SenderName = senderName;
            SendedAt = sendedAt;
            Content = content;
            Edited = edited;
        }
    }
}
