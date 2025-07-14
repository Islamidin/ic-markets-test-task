using ICMarkets.Domain.Entities;
using ICMarkets.Domain.Enums;

namespace ICMarkets.Domain.Interfaces;

public interface IBlockchainRepository
{
    Task AddAsync(BlockchainData data, CancellationToken cancellationToken);

    Task<IReadOnlyList<BlockchainData>> GetHistoryAsync(BlockchainType blockchainType, CancellationToken cancellationToken);
}