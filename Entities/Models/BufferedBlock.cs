using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class BufferedBlock
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
