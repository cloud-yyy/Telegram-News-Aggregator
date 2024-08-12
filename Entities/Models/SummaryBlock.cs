using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class SummaryBlock
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "SummaryId is a required field.")]
        public Guid SummaryId { get; set; }
        public Summary Summary { get; set; }

        [Required(ErrorMessage = "SummaryId is a required field.")]
        public Guid MessageId { get; set; }
        public Message Message { get; set; }
    }
}