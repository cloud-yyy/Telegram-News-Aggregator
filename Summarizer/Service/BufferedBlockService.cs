using Entities.Exceptions;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Summarizer.Service
{
    internal class BufferedBlockService
    {
        private readonly ApplicationContext _context;

        public BufferedBlockService(IDbContextFactory<ApplicationContext> contextFactory)
        {
            _context = contextFactory.CreateDbContext();
        }

        public async Task<Guid?> GetBlockIdForAnyMessageAsync(IEnumerable<Guid> messageIds)
        {
            var block = await _context.BufferedMessages
                .FirstOrDefaultAsync(b => messageIds.Contains(b.MessageId));

            return block?.BlockId;
        }

        public async Task<Guid> CreateBlockAsync()
        {
            var block = new BufferedBlock()
            {
                Id = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow
            };

            _context.BufferedBlocks.Add(block);
            await _context.SaveChangesAsync();

            return block.Id;
        }

        public async Task AddMessageToBlockAsync(Guid blockId, Guid messageId)
        {
            var exists = await _context.BufferedMessages.AnyAsync(m => m.MessageId == messageId);

            if (!exists)
            {
                var block = await _context.BufferedBlocks
                    .Include(b => b.Messages)
                    .SingleOrDefaultAsync(b => b.Id == blockId);

                if (block == null)
                    throw new BufferedBlockNotFoundException(blockId);

                var message = new BufferedMessage()
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    BlockId = blockId
                };

                _context.BufferedMessages.Add(message);

                block.Messages.Add(message);
                block.Size++;
                block.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBlock(Guid blockId)
        {
            var block = _context.BufferedBlocks.Find(blockId);

            if (block != null)
            {
                _context.BufferedBlocks.Remove(block);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetBlockSize(Guid blockId)
        {
            return await _context.BufferedMessages
                .Where(m => m.BlockId == blockId)
                .CountAsync();
        }
    }
}
