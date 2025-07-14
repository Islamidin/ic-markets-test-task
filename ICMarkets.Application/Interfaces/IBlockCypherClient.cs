using ICMarkets.Domain.Enums;

namespace ICMarkets.Application.Interfaces;

public interface IBlockCypherClient
{
    Task<string> GetBlockchainInfoAsync(BlockchainType type, CancellationToken cancellationToken);
}