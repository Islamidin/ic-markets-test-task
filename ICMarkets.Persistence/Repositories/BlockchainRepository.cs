using ICMarkets.Domain.Entities;
using ICMarkets.Domain.Enums;
using ICMarkets.Domain.Interfaces;
using ICMarkets.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Persistence.Repositories;

public class BlockchainRepository : IBlockchainRepository
{
    private readonly AppDbContext context;

    public BlockchainRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(BlockchainData data, CancellationToken cancellationToken)
    {
        await context.BlockchainData.AddAsync(data, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockchainData>> GetHistoryAsync(BlockchainType blockchainType, CancellationToken cancellationToken)
    {
        return await context.BlockchainData
                            .Where(x => x.Blockchain == blockchainType)
                            .OrderByDescending(x => x.CreatedAt)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
    }
}