namespace Entities.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public long TelegramId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime SubscribtionsUpdatedAt { get; set; }

        public ICollection<Channel> Subscribtions { get; set; } = [];
    }
}
