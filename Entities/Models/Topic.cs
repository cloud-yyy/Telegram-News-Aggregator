namespace Entities.Models
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public ICollection<Channel>? Channels { get; set; }
        public ICollection<User>? Subscribers { get; set; }
    }
}
