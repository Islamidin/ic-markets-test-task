using AutoMapper;
using ICMarkets.Application.DTO;
using ICMarkets.Domain.Interfaces;
using MediatR;

namespace ICMarkets.Application.Queries.GetBlockchainHistory;

public class GetBlockchainHistoryHandler : IRequestHandler<GetBlockchainHistoryQuery, IReadOnlyList<BlockchainDataDto>>
{
    private readonly IMapper mapper;
    private readonly IBlockchainRepository repository;

    public GetBlockchainHistoryHandler(IBlockchainRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public async Task<IReadOnlyList<BlockchainDataDto>> Handle(GetBlockchainHistoryQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetHistoryAsync(request.BlockchainType, cancellationToken);
        return mapper.Map<IReadOnlyList<BlockchainDataDto>>(entities);
    }
}