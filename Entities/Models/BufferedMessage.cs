namespace Entities.Models
{
    public class BufferedMessage
    {
        public Guid Id { get; set; }

        public Guid BlockId { get; set; }
        public BufferedBlock? Block { get; set; }

        public Guid MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
