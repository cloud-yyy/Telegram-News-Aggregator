namespace TelegramNewsAggregator
{
    public record SummarizedMessageDto
    {
        public long Id { get; set; }
        public string Title { get; init; }
        public string SummarizedContent { get; init; }
        public string OriginMessageReference { get; init; }

        public SummarizedMessageDto(long id, string title, string summarizedContent, string originMessageReference)
        {
            Id = id;
            Title = title;
            SummarizedContent = summarizedContent;
            OriginMessageReference = originMessageReference;
        }
    }
}