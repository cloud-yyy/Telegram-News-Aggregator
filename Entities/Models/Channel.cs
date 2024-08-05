using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Channel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "TelegramId is a required field.")]
        public long TelegramId { get; set; }
        
        [MinLength(5, ErrorMessage = "Minimum length of Name is 5.")]
        [MaxLength(32, ErrorMessage = "Maximum length of Name is 32.")]
        public string? Name { get; set; }
    }
}