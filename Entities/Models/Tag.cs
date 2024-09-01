namespace Entities.Models;

public class Tag
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public ICollection<Message>? Messages { get; set; } = [];
}
