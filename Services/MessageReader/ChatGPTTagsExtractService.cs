using Entities.Exceptions;
using Newtonsoft.Json;
using OpenAI.Chat;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class ChatGPTTagsExtractService : ITagsExtractService
    {
        private readonly ChatGptClient _client;
        private readonly TagsDbWriter _tagsDbWriter;
        private readonly SemaphoreSlim _semaphoreSlim;

        public ChatGPTTagsExtractService(ChatGptClient client, TagsDbWriter tagsDbWriter)
        {
            _client = client;
            _tagsDbWriter = tagsDbWriter;
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public async Task<MessageTagsDto> ExtractTagsAsync(MessageDto message)
        {
            await _semaphoreSlim.WaitAsync();
            
            try
            {
                return await ExtractAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new TagsExtractionException(ex);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<MessageTagsDto> ExtractAsync(MessageDto message)
        {
            var text = message.Content;

            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"Cannot extract tags from null-valued or empty string {nameof(text)}");

            ChatCompletion result = await _client.Client.CompleteChatAsync($"{_client.Params.ExtractKeywordsPrompt}\n{message.Content}");

            var tagsJson = ChatGptClient.TrimJsonResponse(result.ToString(), '[');
            var tags = JsonConvert.DeserializeObject<IEnumerable<string>>(tagsJson);

            if (tags == null)
                throw new ArgumentNullException($"Argument {tags} cannot be null.");

            var tagsForMessage = new MessageTagsDto(message.Id, tags);
            return tagsForMessage;
        }        
    }
}