using AutoMapper;
using Entities.Exceptions;
using Entities.Models;
using Repository;
using Shared.Dtos;

namespace TelegramNewsAggregator
{
    public class ChannelService
    {
        private readonly ChannelRepository _repository;
        private readonly ITelegramChannelIdResolver _channelIdResolver;
        private readonly IMapper _mapper;

        public ChannelService(ChannelRepository repository, ITelegramChannelIdResolver channelIdResolver, IMapper mapper)
        {
            _repository = repository;
            _channelIdResolver = channelIdResolver;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChannelDto>> GetAllChannelsAsync()
        {
            var channels = await _repository.GetAllChannelsAsync();

            var dtos = channels
                .Select(c => new ChannelDto(c.Id, c.TelegramId, c.Name));

            return dtos;
        }

        public async Task<ChannelDto> GetChannelAsync(Guid id)
        {
            var channel = await _repository.GetChannelAsync(id);

            if (channel == null)
                throw new ChannelNotFoundException(id);

            return new ChannelDto(channel.Id, channel.TelegramId, channel.Name);
        }

        public async Task<ChannelDto> GetChannelByTelegramIdAsync(long telegramId)
        {
            var channel = await _repository.GetChannelByTelegramIdAsync(telegramId);

            if (channel == null)
                throw new ChannelNotFoundException(telegramId);

            return new ChannelDto(channel.Id, channel.TelegramId, channel.Name);
        }

        public async Task<ChannelDto> CreateChannelAsync(ChannelForCreationDto channelForCreationDto)
        {
            if (channelForCreationDto == null)
                throw new ArgumentNullException(nameof(channelForCreationDto));

            if (channelForCreationDto.Id == null && channelForCreationDto.Tag == null)
                throw new ArgumentException(nameof(channelForCreationDto));

            var channel = new Channel()
            {
                Id = new Guid(),
                TelegramId = channelForCreationDto.Id ?? (await _channelIdResolver.ResolveByTag(channelForCreationDto.Tag)),
                Name = channelForCreationDto.Tag,
            };

            _repository.CreateChannel(channel);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ChannelDto>(channel);
        }

        public async Task DeleteChannelAsync(Guid id)
        {
            var channel = await _repository.GetChannelAsync(id);

            if (channel == null)
                throw new ChannelNotFoundException(id);

            _repository.DeleteChannel(channel);
            await _repository.SaveChangesAsync();
        }
    }
}
