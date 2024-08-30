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
                .Where(b => messageIds.Contains(b.MessageId))
                .FirstOrDefaultAsync();

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
                var message = new BufferedMessage()
                {
                    Id = Guid.NewGuid(),
                    MessageId = messageId,
                    BlockId = blockId
                };

                _context.BufferedMessages.Add(message);

                var currentSize = await _context.BufferedBlocks
                    .Where(b => b.Id == blockId)
                    .Select(b => b.Size)
                    .SingleAsync();

                await _context.BufferedBlocks
                    .Where(b => b.Id == blockId)
                    .ExecuteUpdateAsync(s =>
                        s.SetProperty(b => b.Size, currentSize + 1));

                var block = await _context.BufferedBlocks.FindAsync(blockId);
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
