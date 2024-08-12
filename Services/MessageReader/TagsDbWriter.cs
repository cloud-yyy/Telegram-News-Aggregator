using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class TagsDbWriter
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

        public async Task SaveTagsAsync(MessageTagsDto messageTags)
        {
            await _semaphore.WaitAsync();

            try
            {
                await CreateNonExistentTags(messageTags.Tags);
                await CreateMessageTags(messageTags.MessageId, messageTags.Tags);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task CreateNonExistentTags(IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (!await _context.Tags!.AnyAsync(t => t.Name == tag))
                {
                    var entity = new Tag() { Name = tag };
                    _context.Tags!.Add(entity);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task CreateMessageTags(Guid messageId, IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                var entity = await _context.Tags!.SingleOrDefaultAsync(t => t.Name == tag);
                var messageTag = new Entities.MessageTag() { MessageId = messageId, TagId = entity!.Id };
                _context.MessagesTags!.Add(messageTag);
            }

            await _context.SaveChangesAsync();
        }
    }
}
