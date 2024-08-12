using AutoMapper;
using Entities.Models;
using Repository;
using Shared.Dtos;

namespace Services
{
    public class SummaryDbWriter
    {
        private readonly RepositoryContext _context;
        private readonly IMapper _mapper;
        private readonly SemaphoreSlim _semaphore;

        public SummaryDbWriter(RepositoryContextFactory contextFactory, IMapper mapper)
        {
            _context = contextFactory.Create();
            _mapper = mapper;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task SaveSummaryAsync(SummaryDto dto)
        {
            await _semaphore.WaitAsync();

            try
            {
                await SaveAsync(dto);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SaveAsync(SummaryDto dto)
        {
            var summary = _mapper.Map<Summary>(dto);
            _context.Summaries.Add(summary);

            foreach (var messageId in dto.Sources)
            {
                var summaryBlock = new SummaryBlock()
                {
                    Id = Guid.NewGuid(),
                    SummaryId = summary.Id,
                    MessageId = messageId
                };

                _context.SummaryBlocks.Add(summaryBlock);
            }

            await _context.SaveChangesAsync();
        }
    }
}
