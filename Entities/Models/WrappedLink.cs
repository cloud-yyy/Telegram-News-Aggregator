namespace Entities.Models
{
    public class WrappedLink
    {
        public Guid Id { get; set; }
        public string InnerLink { get; set; } = string.Empty;
    }
}
