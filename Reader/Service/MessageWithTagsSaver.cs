using System.Collections.Concurrent;
using AutoMapper;
using Entities.Models;
using MessageBroker.Contracts;
using MessageBroker.Service;
using Microsoft.EntityFrameworkCore;
using Reader.Contracts;
using Repository;
using Shared.Dtos;
using Shared.Notifications;

namespace Reader.Service;

internal class MessageWithTagsSaver : IMessageSaver
{
    private readonly ITagsExtractService _extractService;
    private readonly Broker _broker;
    private readonly ApplicationContext _context;
    private readonly IMapper _mapper;

    private readonly BlockingCollection<MessageDto> _messagesQueue;
    private readonly Task _executionTask;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public MessageWithTagsSaver(
        ITagsExtractService extractService,
        IDbContextFactory<ApplicationContext> contextFactory,
        Broker broker,
        IMapper mapper)
    {
        _extractService = extractService;
        _broker = broker;
        _context = contextFactory.CreateDbContext();
        _mapper = mapper;

        _messagesQueue = new();
        _cancellationTokenSource = new();
        _executionTask = Task.Factory.StartNew
            (Process, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Save(MessageDto message)
    {
        _messagesQueue.Add(message);
    }

    private async Task Process()
    {
        foreach (var message in _messagesQueue.GetConsumingEnumerable())
        {
            var tags = await _extractService.ExtractTagsAsync(message);

            await SaveMessageAsync(message);
            await CreateNonExistentTags(tags);
            await CreateMessageTags(message.Id, tags);

            _broker.Push(new MessageCreatedNotification(message.Id));
        }
    }

    private async Task SaveMessageAsync(MessageDto message)
    {
        var model = _mapper.Map<Message>(message);
        var channelModel = _context.Channels.SingleOrDefault(c => c.Id == message.ChannelId);
        model.Channel = channelModel;

        _context.Messages.Add(model);
        await _context.SaveChangesAsync();
    }

    private async Task CreateNonExistentTags(IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            if (!await _context.Tags!.AnyAsync(t => t.Name == tag))
            {
                var entity = new Tag() { Name = tag };
                _context.Tags!.Add(entity);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task CreateMessageTags(Guid messageId, IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            var entity = await _context.Tags!.SingleOrDefaultAsync(t => t.Name == tag);
            var messageTag = new MessageTag() { MessageId = messageId, TagId = entity!.Id };
            _context.MessagesTags!.Add(messageTag);
        }

        await _context.SaveChangesAsync();
    }
}
