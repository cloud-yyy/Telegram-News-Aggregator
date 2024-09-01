using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class SummaryEntityTypeConfiguration : IEntityTypeConfiguration<Summary>
    {
        public void Configure(EntityTypeBuilder<Summary> builder)
        {
            builder
                .Property(s => s.CreatedAt)
                .IsRequired();

            builder
                .Property(s => s.Title)
                .IsRequired();

            builder
                .Property(s => s.Content)
                .IsRequired();

            builder
                .HasMany(s => s.Sources)
                .WithMany()
                .UsingEntity<SummaryBlock>();
        }
    }
}
