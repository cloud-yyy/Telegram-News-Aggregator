namespace Shared.Params
{
    public class ChatGPTParams
    {
        public string Token { get; set; }
        public string ModelVersion { get; set; }
        public string SummarizeSinglePrompt { get; set; }
        public string SummarizeManyPrompt { get; set; }
        public string ExtractKeywordsPrompt { get; set; }
        public string ComparePrompt { get; set; }
        public string TitleSummarySeparator { get; set; }
        public string MessagesForSummarizeSeparator { get; set; }

        public ChatGPTParams(
            string token, 
            string modelVersion, 
            string summarizeSinglePrompt,
            string summarizeManyPrompt,
            string extractKeywordsPrompt,
            string comparePrompt,
            string titleSummarySeparator,
            string messagesForSummarizeSeparator)
        {
            Token = token;
            ModelVersion = modelVersion;
            SummarizeSinglePrompt = summarizeSinglePrompt;
            SummarizeManyPrompt = summarizeManyPrompt;
            ExtractKeywordsPrompt = extractKeywordsPrompt;
            ComparePrompt = comparePrompt;
            TitleSummarySeparator = titleSummarySeparator;
            MessagesForSummarizeSeparator = messagesForSummarizeSeparator;
        }
    }
}