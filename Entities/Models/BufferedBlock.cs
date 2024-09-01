namespace Entities.Models
{
    public class BufferedBlock
    {
        public Guid Id { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int Size { get; set; }

        public ICollection<BufferedMessage> Messages { get; set; } = [];
    }
}
