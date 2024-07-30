namespace TelegramNewsAggregator
{
    public record SummarizedMessageDto
    {
        public long Id { get; set; }
        public string Title { get; init; }
        public string SummarizedContent { get; init; }
        public string SourceMessageReference { get; init; }

        public SummarizedMessageDto(long id, string title, string summarizedContent, string sourceMessageReference)
        {
            Id = id;
            Title = title;
            SummarizedContent = summarizedContent;
            SourceMessageReference = sourceMessageReference;
        }
    }
}