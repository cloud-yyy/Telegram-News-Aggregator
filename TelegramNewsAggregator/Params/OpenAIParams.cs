namespace TelegramNewsAggregator
{
    public class OpenAIModelParams
    {
        public string? Token { get; set; }
        public string? ModelVersion { get; set; }
        public string? SummarizePrompt { get; set; }
        public string? TitleSummarySeparator { get; set; }

        public OpenAIModelParams(string? token, string? modelVersion, string? summarizePrompt, string? titleSummarySeparator)
        {
            Token = token;
            ModelVersion = modelVersion;
            SummarizePrompt = summarizePrompt;
            TitleSummarySeparator = titleSummarySeparator;
        }
    }
}