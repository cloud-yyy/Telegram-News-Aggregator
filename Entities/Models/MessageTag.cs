namespace Entities.Models;

public class MessageTag
{
	public Guid Id { get; set; }

	public Guid MessageId { get; set; }
	public Message? Message { get; set; }

	public Guid TagId { get; set; }
	public Tag? Tag { get; set; }
}
