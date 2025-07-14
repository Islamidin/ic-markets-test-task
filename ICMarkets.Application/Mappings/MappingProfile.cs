using AutoMapper;
using ICMarkets.Application.DTO;
using ICMarkets.Domain.Entities;

namespace ICMarkets.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BlockchainData, BlockchainDataDto>();
    }
}