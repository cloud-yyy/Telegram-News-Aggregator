namespace Entities.Models
{
    public class Message
    {
        public enum SummarizationStatus
        {
            NotSummarized,
            SummarizedSingle,
            SummarizedMultiple,
            InBlock
        }

        public Guid Id { get; set; }

        public long TelegramId { get; set; }

        public DateTime SendedAt { get; set; }

        public string Content { get; set; }

        public string Uri { get; set; }

        public SummarizationStatus Status { get; set; }

        public Guid ChannelId { get; set; }
        public Channel? Channel { get; set; }

        public ICollection<Tag> Tags { get; set; } = [];
    }
}
