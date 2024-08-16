using AutoMapper;
using Entities.Models;
using Shared.Dtos;

namespace TelegramNewsAggregator
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Channel, ChannelDto>();

            CreateMap<Message, MessageDto>()
                .ReverseMap();

            CreateMap<Summary, SummaryDto>()
                .ReverseMap();

            CreateMap<BufferedMessage, BufferedMessageDto>();
        }
    }
}
