using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class UserTopicEntityTypeConfiguration : IEntityTypeConfiguration<UserTopic>
    {
        public void Configure(EntityTypeBuilder<UserTopic> builder)
        {
            builder
                .HasIndex(e => new { e.UserId, e.TopicId })
                .IsUnique();
        }
    }
}
