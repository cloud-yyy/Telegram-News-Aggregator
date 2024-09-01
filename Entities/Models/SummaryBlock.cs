namespace Entities.Models
{
    public class SummaryBlock
    {
        public Guid Id { get; set; }

        public Guid SummaryId { get; set; }
        public Summary? Summary { get; set; }
        
        public Guid MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
