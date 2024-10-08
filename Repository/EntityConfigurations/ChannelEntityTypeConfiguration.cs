using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class ChannelEntityTypeConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder
                .Property(c => c.Name)
                .IsRequired();

            builder
               .HasIndex(c => c.TelegramId)
               .IsUnique();

            builder
                .HasMany(c => c.Subscribers)
                .WithMany(c => c.Subscribtions)
                .UsingEntity<UserChannel>();
        }
    }
}
