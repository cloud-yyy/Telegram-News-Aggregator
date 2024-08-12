namespace Shared.Dtos
{
    public record MessagesComparingResultDto
    {
        public record ComparedMessagesBlock
        {
            public List<Guid> MessagesIds { get; init; }

            public ComparedMessagesBlock(List<Guid> messagesIds)
            {
                MessagesIds = messagesIds;
            }

            public bool IsSingle() => MessagesIds.Count == 1;
        }

        public IEnumerable<ComparedMessagesBlock> Results { get; init; }

        public MessagesComparingResultDto(IEnumerable<ComparedMessagesBlock> results)
        {
            Results = results;
        }
    }
}
