using AutoMapper;
using Entities.Models;
using Repository;
using Shared.Dtos;

namespace Services;

public class MessageDbWriter
{
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly SemaphoreSlim _semaphore;

	public MessageDbWriter(ApplicationContextFactory contextFactory, IMapper mapper)
	{
		_context = contextFactory.Create();
		_mapper = mapper;
		_semaphore = new(1, 1);
	}

    public async Task SaveMessageAsync(MessageDto message)
    {
		await _semaphore.WaitAsync();

		try
		{
			await SaveAsync(message);
		}
		finally
		{
			_semaphore.Release();
		}
	}

	private async Task SaveAsync(MessageDto message)
	{
		var model = _mapper.Map<Message>(message);
		var channelModel = _context.Channels.SingleOrDefault(c => c.Id == message.ChannelId);
		model.Channel = channelModel;

		_context.Messages.Add(model);
		await _context.SaveChangesAsync();
	}
}
