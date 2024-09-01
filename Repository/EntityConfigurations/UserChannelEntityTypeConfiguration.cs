using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class UserChannelEntityTypeConfiguration : IEntityTypeConfiguration<UserChannel>
    {
        public void Configure(EntityTypeBuilder<UserChannel> builder)
        {
            builder
                .HasIndex(e => new { e.UserId, e.ChannelId })
                .IsUnique();
        }
    }
}
