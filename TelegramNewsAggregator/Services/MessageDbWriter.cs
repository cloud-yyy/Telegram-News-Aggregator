using AutoMapper;
using Entities.Models;
using Repository;
using Shared.Dtos;

namespace TelegramNewsAggregator;

public class MessageDbWriter : IMessageConsumer<MessageDto>
{
	private readonly MessageRepository _messageRepository;
    private readonly ChannelRepository _channelRepository;
    private readonly IMapper _mapper;

	public MessageDbWriter(MessageRepository messageRepository, ChannelRepository channelRepository, IMapper mapper)
	{
		_messageRepository = messageRepository;
		_channelRepository = channelRepository;
		_mapper = mapper;
	}

    public async Task Notify(MessageDto message)
    {
		var model = _mapper.Map<Message>(message);
		var channelModel = await _channelRepository.GetChannelAsync(message.ChannelId);
		model.Channel = channelModel;
		
		_messageRepository.CreateMessage(model);
		await _messageRepository.SaveChangesAsync();
    }
}
