using ICMarkets.Domain.Enums;
using MediatR;

namespace ICMarkets.Application.Commands.FetchBlockchainData;

public record FetchBlockchainDataCommand(BlockchainType Blockchain) : IRequest<Unit>;