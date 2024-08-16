using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Message
    {
        public enum SummarizationStatus
        {
            NotSummarized,
            SummarizedSingle,
            SummarizedMultiple,
            InBlock
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "TelegramId is a required field.")]
        public long TelegramId { get; set; }

        [Required(ErrorMessage = $"SendedAt is required field")]
        public DateTime SendedAt { get; set; }

        [Required(ErrorMessage = $"Content is required field")]
        public string Content { get; set; }

        [Required(ErrorMessage = $"Uri is required field")]
        public string Uri { get; set; }

        public Guid ChannelId { get; set; }
        public Channel? Channel { get; set; }

        [EnumDataType(typeof(SummarizationStatus))]
        [DefaultValue(SummarizationStatus.NotSummarized)]
        public SummarizationStatus Status { get; set; }
    }
}
