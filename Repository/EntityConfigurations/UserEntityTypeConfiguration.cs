using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasIndex(u => u.TelegramId)
                .IsUnique();

            builder
                .Property(u => u.CreatedAt)
                .IsRequired();

            builder
                .Property(u => u.SubscribtionsUpdatedAt)
                .IsRequired();

            builder.HasData
            (
                new User()
                {
                    Id = Guid.NewGuid(),
                    TelegramId = 1268190565,
                    CreatedAt = DateTime.UtcNow,
                }
            );
        }
    }
}