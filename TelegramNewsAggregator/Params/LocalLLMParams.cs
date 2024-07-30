namespace TelegramNewsAggregator
{
    public class LocalLLMParams
    {
        public string? ModelPath { get; set; }
        public string? SummarizePrompt { get; set; }
        public string? TitleSummarySeparator { get; set; }
    }
}