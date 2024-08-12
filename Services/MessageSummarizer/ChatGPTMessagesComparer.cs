using Entities.Exceptions;
using Newtonsoft.Json;
using OpenAI.Chat;
using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class ChatGPTMessagesComparer : IMessageComparer
    {
        private readonly ChatGptClient _client;

        public ChatGPTMessagesComparer(ChatGptClient client)
        {
            _client = client;
        }

        public async Task<MessagesComparingResultDto> CompareByTags(IEnumerable<MessageTagsDto> messageTags)
        {
            var jsonData = JsonConvert.SerializeObject(messageTags);
            var prompt = $"{_client.Params.ComparePrompt}:\n{jsonData}";

            try
            {
                ChatCompletion completion = await _client.Client.CompleteChatAsync(prompt);
                var jsonResult = ChatGptClient.TrimJsonResponse(completion.ToString());
                var result = JsonConvert.DeserializeObject<MessagesComparingResultDto>(jsonResult);

                if (result == null)
                    throw new NullReferenceException();                
                
                return result;
            }
            catch (Exception ex)
            {
                throw new MessagesComparingException(ex.ToString());
            }
        }
    }
}
