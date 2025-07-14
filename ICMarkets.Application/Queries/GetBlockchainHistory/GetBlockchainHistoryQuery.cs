using ICMarkets.Application.DTO;
using ICMarkets.Domain.Enums;
using MediatR;

namespace ICMarkets.Application.Queries.GetBlockchainHistory;

public record GetBlockchainHistoryQuery(BlockchainType BlockchainType) : IRequest<IReadOnlyList<BlockchainDataDto>>;