using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class TopicEntityTypeConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.Property(t => t.Name).IsRequired();

            builder
                .HasMany(t => t.Channels)
                .WithMany(c => c.Topics)
                .UsingEntity<TopicChannel>();

            builder
                .HasMany(t => t.Subscribers)
                .WithMany(u => u.SubscribedTopics)
                .UsingEntity<UserTopic>();
        }
    }
}
