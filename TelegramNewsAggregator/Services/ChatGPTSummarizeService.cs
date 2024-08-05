using Entities.Exceptions;
using OpenAI.Chat;
using Shared.Dtos;

namespace TelegramNewsAggregator
{
    public class ChatGPTSummarizeService : ISummarizeService, IMessageConsumer<MessageDto>
    {
        private readonly OpenAIModelParams _modelParameters;
        private readonly ChatClient _client;
        private readonly MessageBroker _broker;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;

        public ChatGPTSummarizeService(MessageBroker broker, ILogger logger, IConfiguration configuration)
        {
            var paramsSection = configuration.GetSection("OpenAIParams");
            var token = Environment.GetEnvironmentVariable("openai_token");

            if (paramsSection == null)
                throw new ConfigurationNotFoundException("OpenAIParams");

            if (token == null)
                throw new EnviromentVariableNotFoundException("openai_token");

            _modelParameters = new
            (
                token: token,
                modelVersion: paramsSection["ModelVersion"],
                summarizePrompt: paramsSection["SummarizePrompt"],
                titleSummarySeparator: paramsSection["TitleSummarySeparator"]
            );

            _client = new(_modelParameters.ModelVersion, token);
            _broker = broker;
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Notify(MessageDto message)
        {
            await Summarize(message);
        }

        public async Task Summarize(MessageDto message)
        {
            await _semaphore.WaitAsync();

            _logger.LogInfo($"ChatGptSummarizeService started for {message.Id} in thread: {Environment.CurrentManagedThreadId}");

            var (title, summary) = await TrySummarize(message);

            var dto = new SummarizedMessageDto
            (
                id: message.Id,
                telegramId: message.TelegramId,
                title: title,
                summarizedContent: summary,
                sourceMessageReference: "mock"
            );

            _logger.LogInfo($"ChatGptSummarizeService finished for {message.Id} in thread: {Environment.CurrentManagedThreadId}");

            _broker.Push(dto);
            _semaphore.Release();
        }

        private async Task<(string title, string summary)> TrySummarize(MessageDto message)
        {
            try
            {
                var prompt = $"{_modelParameters.SummarizePrompt} {message.Content}";

                ChatCompletion completion = await _client.CompleteChatAsync(prompt);
                
                var parts = completion.ToString().Split(_modelParameters.TitleSummarySeparator);
                var title = "Title";
                var summary = completion.ToString();

                if (parts.Length == 2)
                {
                    title = parts[0];
                    summary = parts[1];
                }

                return (title, summary);
            }
            catch (Exception ex)
            {
                throw new SummarizationException(ex.Message);
            }
        }

        private string CreateMessageUri(string channelName, long messageId)
        {
            return $"t.me/{Uri.EscapeDataString(channelName)}/{messageId}";
        }
    }
}