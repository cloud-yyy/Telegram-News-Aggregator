using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Entities.Models;

public class MessageTag
{
	public Guid Id { get; set; }

	[Required(ErrorMessage = "MessageId is a required field.")]
	public Guid MessageId { get; set; }
	public Message? Message { get; set; }

	[Required(ErrorMessage = "TagId is a required field.")]
	public Guid TagId { get; set; }
	public Tag? Tag { get; set; }
}
