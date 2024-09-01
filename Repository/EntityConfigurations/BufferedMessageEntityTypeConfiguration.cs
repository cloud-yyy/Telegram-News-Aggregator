using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class BufferedMessageEntityTypeConfiguration : IEntityTypeConfiguration<BufferedMessage>
    {
        public void Configure(EntityTypeBuilder<BufferedMessage> builder)
        {
            builder
                .HasIndex(e => e.MessageId)
                .IsUnique();
        }
    }
}
