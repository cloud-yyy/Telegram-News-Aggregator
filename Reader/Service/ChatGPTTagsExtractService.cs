using Newtonsoft.Json;
using OpenAI.Chat;
using Reader.Contracts;
using Shared.Clients;
using Shared.Dtos;

namespace Services;

internal class ChatGPTTagsExtractService : ITagsExtractService
{
    private readonly ChatGptClient _client;
    private readonly SemaphoreSlim _semaphoreSlim;

    public ChatGPTTagsExtractService(ChatGptClient client)
    {
        _client = client;
        _semaphoreSlim = new SemaphoreSlim(1, 1);
    }

    public async Task<IEnumerable<string>> ExtractTagsAsync(MessageDto message)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            return await ExtractAsync(message);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task<IEnumerable<string>> ExtractAsync(MessageDto message)
    {
        var text = message.Content;

        if (string.IsNullOrEmpty(text))
            throw new ArgumentException($"Cannot extract tags from null-valued or empty string {nameof(text)}");

        ChatCompletion result = await _client.Client.CompleteChatAsync($"{_client.Params.ExtractKeywordsPrompt}\n{message.Content}");

        var tagsJson = ChatGptClient.TrimJsonResponse(result.ToString(), '[');
        var tags = JsonConvert.DeserializeObject<IEnumerable<string>>(tagsJson);

        if (tags == null)
            throw new ArgumentNullException($"Argument {tags} cannot be null.");

        return tags;
    }
}
