namespace Entities.Models
{
    public class UserChannel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set;}
        public User User { get; set; }

        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; }
    }
}
