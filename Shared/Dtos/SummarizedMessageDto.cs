namespace Shared.Dtos
{
    public record SummarizedMessageDto
    {
        public Guid Id { get; set; }
        public long TelegramId { get; set; }
        public string Title { get; init; }
        public string SummarizedContent { get; init; }
        public string SourceMessageReference { get; init; }

        public SummarizedMessageDto(Guid id, long telegramId, string title, string summarizedContent, string sourceMessageReference)
        {
            Id = id;
            TelegramId = TelegramId;
            Title = title;
            SummarizedContent = summarizedContent;
            SourceMessageReference = sourceMessageReference;
        }
    }
}