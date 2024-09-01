using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class MessageTagEntityTypeConfiguration : IEntityTypeConfiguration<MessageTag>
    {
        public void Configure(EntityTypeBuilder<MessageTag> builder)
        {
            builder
                .HasIndex(e => new { e.MessageId, e.TagId })
                .IsUnique();
        }
    }
}
