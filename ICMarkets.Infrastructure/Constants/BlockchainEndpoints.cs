using ICMarkets.Domain.Enums;

namespace ICMarkets.Infrastructure.Constants;

public static class BlockchainEndpoints
{
    public static readonly IReadOnlyDictionary<BlockchainType, string> Mainnet = new Dictionary<BlockchainType, string>
    {
        [BlockchainType.Btc] = "https://api.blockcypher.com/v1/btc/main",
        [BlockchainType.Eth] = "https://api.blockcypher.com/v1/eth/main",
        [BlockchainType.Ltc] = "https://api.blockcypher.com/v1/ltc/main",
        [BlockchainType.Dash] = "https://api.blockcypher.com/v1/dash/main"
    };
}