using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace Repository
{
    public class RepositoryContext : ThreadSafeDbContext
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MessageTag> MessagesTags { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<SummaryBlock> SummaryBlocks { get; set; }

        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Channel>()
                .HasIndex(e => e.Name)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(e => e.Name)
                .IsUnique();

            modelBuilder.Entity<MessageTag>()
                .HasIndex(e => new { e.MessageId, e.TagId })
                .IsUnique();

            modelBuilder.Entity<SummaryBlock>()
                .HasIndex(e => new { e.SummaryId, e.MessageId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}