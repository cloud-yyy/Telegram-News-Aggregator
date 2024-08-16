using Services.Contracts;
using Shared.Dtos;
using Shared.Notifications;

namespace Services
{
    public class MessageWithTagsSaver : IMessageConsumer<MessageDto>
    {
        private readonly ITagsExtractService _extractService;
        private readonly MessageDbWriter _messageDbWriter;
        private readonly TagsDbWriter _tagsDbWriter;
        private readonly MessageBroker _broker;

        public MessageWithTagsSaver(
            ITagsExtractService extractService, 
            MessageDbWriter messageDbWriter, 
            TagsDbWriter tagsDbWriter,
            MessageBroker broker)
        {
            _extractService = extractService;
            _messageDbWriter = messageDbWriter;
            _tagsDbWriter = tagsDbWriter;
            _broker = broker;
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

            _broker.Push(new MessageSavedNotification(message.Id));
        }
    }
}
