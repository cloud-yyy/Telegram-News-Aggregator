namespace TelegramNewsAggregator
{
    public class LLMParams
    {
        public string? ModelPath { get; set; }
        public string? SummarizePrompt { get; set; }
        public string? TitleSummarySeparator { get; set; }
    }
}