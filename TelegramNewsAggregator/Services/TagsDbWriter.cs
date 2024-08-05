using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Shared.Dtos;

namespace TelegramNewsAggregator.Services
{
    public class TagsDbWriter : IMessageConsumer<TagsForMessageDto>
    {
        private readonly RepositoryContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;

        public TagsDbWriter(RepositoryContextFactory contextFactory, IMapper mapper, ILogger logger)
        {
            _context = contextFactory.Create();
            _mapper = mapper;
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Notify(TagsForMessageDto message)
        {
            await CreateTagsForMessage(message);
        }

        public async Task CreateTagsForMessage(TagsForMessageDto message)
        {
            await _semaphore.WaitAsync();

            try
            {
                _logger.LogInfo("Creating tags started...");

                await CreateNonExistentTags(message.Tags);
                await CreateMessageTags(message.MessageId, message.Tags);

                _logger.LogInfo("Creating tags finished.");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task CreateNonExistentTags(IEnumerable<TagDto> tags)
        {
            foreach (var dto in tags)
            {
                if (!await _context.Tags!.AnyAsync(t => t.Name == dto.Name))
                {
                    var tag = new Tag() { Name = dto.Name };
                    _context.Tags!.Add(tag);
                    await _context.SaveChangesAsync();
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateMessageTags(Guid messageId, IEnumerable<TagDto> tags)
        {
            foreach (var dto in tags)
            {
                var tag = await _context.Tags!.SingleOrDefaultAsync(t => t.Name == dto.Name);
                var messageTag = new MessageTag() { MessageId = messageId, TagId = tag.Id };
                _context.MessagesTags!.Add(messageTag);
            }

            await _context.SaveChangesAsync();
        }
    }
}
