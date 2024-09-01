namespace Entities.Models
{
    public class Summary
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public ICollection<Message> Sources { get; set; } = [];
    }
}
