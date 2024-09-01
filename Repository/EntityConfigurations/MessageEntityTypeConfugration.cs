using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.EntityConfigurations
{
    public class MessageEntityTypeConfugration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .Property(m => m.TelegramId)
                .IsRequired();

            builder
                .Property(m => m.SendedAt)
                .IsRequired();

            builder
                .Property(m => m.Content)
                .IsRequired();

            builder
                .Property(m => m.Uri)
                .IsRequired();

            builder
                .HasMany(e => e.Tags)
                .WithMany(e => e.Messages)
                .UsingEntity<MessageTag>();
        }
    }
}
