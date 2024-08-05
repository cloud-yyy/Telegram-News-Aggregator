using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Summary
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        public string Content { get; set; }
    }
}