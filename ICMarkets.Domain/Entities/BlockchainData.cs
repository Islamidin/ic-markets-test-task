using ICMarkets.Domain.Enums;

namespace ICMarkets.Domain.Entities;

public class BlockchainData
{
    public Guid Id { get; set; }

    public BlockchainType Blockchain { get; set; }

    public string RawJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}