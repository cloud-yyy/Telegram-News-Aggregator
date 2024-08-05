using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Tag
{
	public Guid Id { get; set; }

	[Required(ErrorMessage = "Name is a required field.")]
	public string Name { get; set; }
}
