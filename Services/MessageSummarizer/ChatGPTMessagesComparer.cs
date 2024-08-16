using Entities.Exceptions;
using Newtonsoft.Json;
using OpenAI.Chat;
using Repository;
using Shared.Dtos;

namespace Services
{
    public class ChatGPTMessagesComparer : MessageComparerBase
    {
        private readonly ChatGptClient _client;

        public ChatGPTMessagesComparer(ApplicationContextFactory contextFactory, ChatGptClient client)
            : base(contextFactory)
        {
            _client = client;
        }

        protected override async Task<IEnumerable<Guid>> CompareToMessageAsync
            (MessageTagsDto newMessageTags, IEnumerable<MessageTagsDto> existedMessagesTags)
        {
            var newMessageTagsJson = JsonConvert.SerializeObject(newMessageTags.Tags);
            var jsonData = JsonConvert.SerializeObject(existedMessagesTags);
            var prompt =
                $"{_client.Params.ComparePrompt}\nNew message tags: {newMessageTagsJson}.\nExisted messages tags: {jsonData}";

            try
            {
                ChatCompletion completion = await _client.Client.CompleteChatAsync(prompt);
                var jsonResult = ChatGptClient.TrimJsonResponse(completion.ToString(), '[');
                var result = JsonConvert.DeserializeObject<IEnumerable<Guid>>(jsonResult);

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
