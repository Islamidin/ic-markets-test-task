using ICMarkets.Application.Interfaces;
using ICMarkets.Domain.Enums;
using ICMarkets.Infrastructure.Constants;
using Microsoft.Extensions.Logging;

namespace ICMarkets.Infrastructure.Services;

public class BlockCypherClient : IBlockCypherClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<BlockCypherClient> logger;

    public BlockCypherClient(HttpClient httpClient, ILogger<BlockCypherClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<string> GetBlockchainInfoAsync(BlockchainType blockchain, CancellationToken cancellationToken = default)
    {
        if (!BlockchainEndpoints.Mainnet.TryGetValue(blockchain, out var url))
        {
            logger.LogWarning("Unknown blockchain type: {Blockchain}", blockchain);
            throw new ArgumentException("Unsupported blockchain type", nameof(blockchain));
        }

        logger.LogInformation("Fetching data from {Url}", url);

        try
        {
            using var response = await httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("BlockCypher API returned {StatusCode}: {Body}", response.StatusCode, errorBody);

                throw new HttpRequestException(
                    $"BlockCypher API error ({response.StatusCode}): {errorBody}",
                    null,
                    response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogInformation("Successfully fetched data for {Blockchain}", blockchain);
            return content;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Request was cancelled by caller for {Blockchain}", blockchain);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error fetching data for {Blockchain}", blockchain);
            throw;
        }
    }
}