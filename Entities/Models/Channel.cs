using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Channel
    {
        public Guid Id { get; set; }
        
        public long TelegramId { get; set; }

        [MinLength(5), MaxLength(32)]
        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        public ICollection<User>? Subscribers { get; set; } = [];
    }
}
