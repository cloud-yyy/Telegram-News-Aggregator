using System.Text;
using LLama;
using LLama.Common;

namespace TelegramNewsAggregator
{
    public class LocalLLMSummarizeService : ISummarizeService, IMessageConsumer<MessageDto>
    {
        private readonly LocalLLMParams _llmParams;

        private readonly LLamaWeights _model;
        private readonly InferenceParams _inferenceParams;
        private readonly StatelessExecutor _executor;

        private readonly MessageBroker _broker;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;

        public LocalLLMSummarizeService(MessageBroker broker, ILogger logger, IConfiguration configuration)
        {
            var llmParams = configuration.GetSection("LocalLLMParams");

            if (llmParams == null)
                throw new ConfigurationNotFoundException("LocalLLMParams");

            _llmParams = new()
            {
                ModelPath = llmParams["ModelPath"],
                SummarizePrompt = llmParams["SummarizePrompt"],
                TitleSummarySeparator = llmParams["TitleSummarySeparator"]
            };

            var parameters = new ModelParams(_llmParams.ModelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };

            _model = LLamaWeights.LoadFromFile(parameters);
            _executor = new StatelessExecutor(_model, parameters);

            _inferenceParams = new InferenceParams()
            {
                Temperature = 0.6f,
                AntiPrompts = ["Question:", "#", "Question: "],
                MaxTokens = 80
            };

            _logger = logger;
            _broker = broker;

            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task Notify(MessageDto message)
        {
            _logger.LogInfo($"LLM notified in thread: {Environment.CurrentManagedThreadId}");
            await Summarize(message);
        }

        public async Task Summarize(MessageDto message)
        {
            await _semaphore.WaitAsync();
            _logger.LogInfo($"Summarization started in thread: {Environment.CurrentManagedThreadId}");

            var builder = new StringBuilder();
            var prompt = $"Question: {_llmParams.SummarizePrompt} {message.Content} Answer: ";
            var response = _executor.InferAsync(prompt, _inferenceParams);

            await foreach (var text in response)
                builder.Append(text);

            var parts = builder.ToString().Split(_llmParams.TitleSummarySeparator);

            var title = "Title";
            var summary = builder.ToString();

            if (parts.Length == 2)
            {
                title = parts[0];
                summary = parts[1];
            }

            var summarized = new SummarizedMessageDto
            (
                message.Id,
                title,
                summary,
                // TODO: Maybe extract this link to configuration
                $"t.me/{message.SenderName}/{message.Id}"
            );

            _broker.Push(summarized);
            _logger.LogInfo($"Summarization ended in thread: {Environment.CurrentManagedThreadId}");
            _semaphore.Release();
        }
    }
}
