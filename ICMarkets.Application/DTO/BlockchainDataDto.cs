using ICMarkets.Domain.Enums;

namespace ICMarkets.Application.DTO;

public record BlockchainDataDto(BlockchainType Blockchain, string RawJson, DateTime CreatedAt);