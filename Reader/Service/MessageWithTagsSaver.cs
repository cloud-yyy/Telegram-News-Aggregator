using System.Collections.Concurrent;
using AutoMapper;
using Entities.Models;
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
            var messageModel = _mapper.Map<Message>(message);

            _context.Messages.Add(messageModel);

            await CreateTags(messageModel, tags);

            await _context.SaveChangesAsync();

            _broker.Push(new MessageCreatedNotification(message.Id));
        }
    }

    private async Task CreateTags(Message message, IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            var tagModel = await _context.Tags.SingleOrDefaultAsync(t => t.Name == tag);

            if (tagModel == null)
            {
                tagModel = new Tag() { Id = Guid.NewGuid(), Name = tag };
                _context.Tags.Add(tagModel);
            }

            message.Tags.Add(tagModel);
        }
    }
}
