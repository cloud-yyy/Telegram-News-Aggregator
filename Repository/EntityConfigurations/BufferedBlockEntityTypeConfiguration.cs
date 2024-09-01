using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class BufferedBlockEntityTypeConfiguration : IEntityTypeConfiguration<BufferedBlock>
    {
        public void Configure(EntityTypeBuilder<BufferedBlock> builder)
        {
            builder
                .Property(b => b.UpdatedAt)
                .IsRequired();

            builder
                .HasMany(b => b.Messages);
        }
    }
}
