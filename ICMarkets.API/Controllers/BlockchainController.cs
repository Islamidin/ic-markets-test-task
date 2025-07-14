using ICMarkets.Application.Commands.FetchBlockchainData;
using ICMarkets.Application.Queries.GetBlockchainHistory;
using ICMarkets.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ICMarkets.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly IMediator mediator;

    public BlockchainController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("{blockchainType}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Fetch(BlockchainType blockchainType, CancellationToken cancellationToken)
    {
        await mediator.Send(new FetchBlockchainDataCommand(blockchainType), cancellationToken);
        return Ok("Data fetched successfully.");
    }

    [HttpGet("{blockchainType}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> History(BlockchainType blockchainType, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBlockchainHistoryQuery(blockchainType), cancellationToken);
        return Ok(result);
    }
}