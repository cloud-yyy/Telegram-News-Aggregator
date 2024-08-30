using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ThreadSafe;

namespace Repository
{
    public class ApplicationContext : ThreadSafeDbContext
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        
        public DbSet<Tag> Tags { get; set; }
        public DbSet<MessageTag> MessagesTags { get; set; }
        
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<SummaryBlock> SummaryBlocks { get; set; }
        
        public DbSet<BufferedBlock> BufferedBlocks { get; set; }
        public DbSet<BufferedMessage> BufferedMessages { get; set; }
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserChannel> UserChannels { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
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
                .HasIndex(e => e.MessageId )
                .IsUnique();

            modelBuilder.Entity<BufferedMessage>()
                .HasIndex(e => e.MessageId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(e => e.TelegramId)
                .IsUnique();

            modelBuilder.Entity<UserChannel>()
                .HasIndex(e => new { e.UserId, e.ChannelId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
