using System.Text;
using Entities.Exceptions;
using OpenAI.Chat;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class ChatGPTMessagesSummarizer : IMessagesSummarizer
    {
        private readonly ChatGptClient _client;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore;
        
        public ChatGPTMessagesSummarizer(ChatGptClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async Task<SummaryDto> Summarize(List<MessageDto> messages)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (messages.Count == 1)
                    return await SummarizeSingle(messages[0]);
                else
                    return await SummarizeMany(messages);
            }
            catch (Exception ex)
            {
                throw new SummarizationException(ex.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<SummaryDto> SummarizeSingle(MessageDto message)
        {
            var prompt = $"{_client.Params.SummarizeSinglePrompt} {message.Content}";
            return await SummarizeFromPrompt(prompt, [message]);
        }

        private async Task<SummaryDto> SummarizeMany(List<MessageDto> messages)
        {
            var promptBuilder = new StringBuilder(_client.Params.SummarizeManyPrompt);

            foreach (var message in messages)
            {
                promptBuilder.AppendLine(message.Content);
                promptBuilder.AppendLine(_client.Params.MessagesForSummarizeSeparator);
            }

            return await SummarizeFromPrompt(promptBuilder.ToString(), messages);
        }

        private async Task<SummaryDto> SummarizeFromPrompt(string prompt, IEnumerable<MessageDto> sources)
        {
            _logger.LogInfo($"ChatGPTMessagesSummarizer started in thread: {Environment.CurrentManagedThreadId}");
            
            ChatCompletion completion = await _client.Client.CompleteChatAsync(prompt);

            var parts = completion.ToString().Split(_client.Params.TitleSummarySeparator);
            var title = "Title";
            var summary = completion.ToString();

            if (parts.Length == 2)
            {
                title = parts[0];
                summary = parts[1];
            }

            var dto = new SummaryDto
            (
                id: Guid.NewGuid(),
                title: title,
                content: summary,
                sources: sources.Select(s => s.Id),
                createdAt: DateTime.UtcNow
            );

            _logger.LogInfo($"ChatGPTMessagesSummarizer finished in thread: {Environment.CurrentManagedThreadId}");

            return dto;
        }

        private string CreateMessageUri(string channelName, long messageId)
        {
            return $"t.me/{Uri.EscapeDataString(channelName)}/{messageId}";
        }
    }
}