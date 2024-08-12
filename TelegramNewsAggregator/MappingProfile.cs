using AutoMapper;
using Entities;
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
        }
    }
}
