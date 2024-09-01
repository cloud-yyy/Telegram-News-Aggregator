using System.Reflection;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ThreadSafe;
using Repository.EntityConfigurations;

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
            ApplyEntitiesConfigurations(modelBuilder);
            
            base.OnModelCreating(modelBuilder);
        }

        private void ApplyEntitiesConfigurations(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var entityTypeConfigurationTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                .ToList();

            foreach (var type in entityTypeConfigurationTypes)
            {
                var instance = Activator.CreateInstance(type);

                var entityType = type.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    .GetGenericArguments()
                    .First();

                var entityMethod = typeof(ModelBuilder).GetMethod(nameof(ModelBuilder.Entity), [])!
                    .MakeGenericMethod(entityType);

                var entityTypeBuilder = entityMethod.Invoke(modelBuilder, null);

                var configureMethod = type.GetMethod(nameof(IEntityTypeConfiguration<object>.Configure));
                configureMethod!.Invoke(instance, [entityTypeBuilder]);
            }
        }
    }
}
