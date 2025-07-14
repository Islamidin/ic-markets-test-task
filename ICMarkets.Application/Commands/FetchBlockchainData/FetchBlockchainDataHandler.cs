using ICMarkets.Application.Interfaces;
using ICMarkets.Domain.Entities;
using ICMarkets.Domain.Interfaces;
using MediatR;

namespace ICMarkets.Application.Commands.FetchBlockchainData;

public class FetchBlockchainDataHandler : IRequestHandler<FetchBlockchainDataCommand, Unit>
{
    private readonly IBlockCypherClient client;
    private readonly IBlockchainRepository repository;

    public FetchBlockchainDataHandler(IBlockCypherClient client,
                                      IBlockchainRepository repository)
    {
        this.client = client;
        this.repository = repository;
    }

    public async Task<Unit> Handle(FetchBlockchainDataCommand request, CancellationToken cancellationToken)
    {
        var json = await client.GetBlockchainInfoAsync(request.Blockchain, cancellationToken);

        var entity = new BlockchainData
        {
            Id = Guid.NewGuid(),
            Blockchain = request.Blockchain,
            RawJson = json,
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(entity, cancellationToken);

        return Unit.Value;
    }
}