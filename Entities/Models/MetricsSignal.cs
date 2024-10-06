namespace Entities.Models
{
    public class MetricsSignal
    {
        public enum Type
        {
            LandingGoToBotClicked,
            NewUserEnteredBot,
            InterestsSetupFinished,
            ClickedOriginLink
        }

        public Guid Id { get; set; }
        public long UserTelegramId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime ClickedAt { get; set; }
    }
}
