using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class SummaryBlockEntityTypeConfiguration : IEntityTypeConfiguration<SummaryBlock>
    {
        public void Configure(EntityTypeBuilder<SummaryBlock> builder)
        {
            builder
                .Property(sb => sb.SummaryId)
                .IsRequired();

            builder
                .HasIndex(e => e.MessageId)
                .IsUnique();
        }
    }
}
