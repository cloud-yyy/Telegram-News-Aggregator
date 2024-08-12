using Services.Contracts;
using Shared.Dtos;

namespace Services
{
    public class MessageWithTagsSaver : IMessageConsumer<MessageDto>
    {
        private readonly ITagsExtractService _extractService;
        private readonly MessageDbWriter _messageDbWriter;
        private readonly TagsDbWriter _tagsDbWriter;

        public MessageWithTagsSaver(
            ITagsExtractService extractService, 
            MessageDbWriter messageDbWriter, 
            TagsDbWriter tagsDbWriter)
        {
            _extractService = extractService;
            _messageDbWriter = messageDbWriter;
            _tagsDbWriter = tagsDbWriter;
        }

        public async Task Notify(MessageDto message)
        {
            await ExtractTagsAndSaveAsync(message);
        }

        private async Task ExtractTagsAndSaveAsync(MessageDto message)
        {
            var tags = await _extractService.ExtractTagsAsync(message);
            await _messageDbWriter.SaveMessageAsync(message);
            await _tagsDbWriter.SaveTagsAsync(tags);
        }
    }
}
