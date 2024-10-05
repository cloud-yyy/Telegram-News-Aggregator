namespace Entities.Models
{
    public class UserTopic
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid TopicId { get; set; }
        public Topic? Topic { get; set; }
    }
}
