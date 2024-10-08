namespace Shared.Dtos
{
    public record SummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; init; }
        public string Content { get; init; }
        public IEnumerable<MessageDto> Sources { get; init; }
        public DateTime CreatedAt { get; init; }

        public SummaryDto(
            Guid id,
            string title,
            string content,
            IEnumerable<MessageDto> sources,
            DateTime createdAt)
        {
            Id = id;
            Title = title;
            Content = content;
            Sources = sources;
            CreatedAt = createdAt;
        }
    }
}