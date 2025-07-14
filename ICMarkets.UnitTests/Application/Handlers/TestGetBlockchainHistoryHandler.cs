using AutoMapper;
using FluentAssertions;
using ICMarkets.Application.DTO;
using ICMarkets.Application.Queries.GetBlockchainHistory;
using ICMarkets.Domain.Entities;
using ICMarkets.Domain.Enums;
using ICMarkets.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ICMarkets.UnitTests.Application.Handlers;

[TestFixture]
public class TestGetBlockchainHistoryHandler
{
    private const BlockchainType BlockchainBtc = BlockchainType.Btc;
    private GetBlockchainHistoryHandler handler = null!;
    private IMapper mapper = null!;
    private IBlockchainRepository repository = null!;

    [SetUp]
    public void Setup()
    {
        repository = Substitute.For<IBlockchainRepository>();
        mapper = Substitute.For<IMapper>();
        handler = new(repository, mapper);
    }

    [Test]
    public async Task Handle_ShouldReturnMappedDtoList()
    {
        var entityList = new List<BlockchainData>
        {
            new() { Blockchain = BlockchainBtc, RawJson = "{}", CreatedAt = DateTime.UtcNow }
        };

        var dtoList = new List<BlockchainDataDto>
        {
            new(BlockchainBtc, "{}", entityList[0].CreatedAt)
        };

        repository.GetHistoryAsync(BlockchainBtc, Arg.Any<CancellationToken>())
                  .Returns(entityList);

        mapper.Map<IReadOnlyList<BlockchainDataDto>>(entityList)
              .Returns(dtoList);

        var query = new GetBlockchainHistoryQuery(BlockchainBtc);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(dtoList);
        mapper.Received(1).Map<IReadOnlyList<BlockchainDataDto>>(entityList);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenNoHistoryExists()
    {
        var emptyEntities = new List<BlockchainData>();

        repository.GetHistoryAsync(BlockchainBtc, Arg.Any<CancellationToken>())
                  .Returns(emptyEntities);

        mapper.Map<IReadOnlyList<BlockchainDataDto>>(emptyEntities)
              .Returns(new List<BlockchainDataDto>());

        var query = new GetBlockchainHistoryQuery(BlockchainBtc);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
        mapper.Received(1).Map<IReadOnlyList<BlockchainDataDto>>(emptyEntities);
    }

    [Test]
    public void Handle_RepositoryThrows_ShouldPropagateException()
    {
        repository.GetHistoryAsync(BlockchainBtc, Arg.Any<CancellationToken>())
                  .Throws(new Exception("DB is down"));

        var query = new GetBlockchainHistoryQuery(BlockchainBtc);

        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        act.Should().ThrowAsync<Exception>().WithMessage("DB is down");
    }

    [Test]
    public async Task Handle_ShouldReturnDtos_InDescendingOrderByCreatedAt()
    {
        var older = new BlockchainData
        {
            Blockchain = BlockchainBtc,
            RawJson = "{ \"old\": true }",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        var newer = new BlockchainData
        {
            Blockchain = BlockchainBtc,
            RawJson = "{ \"new\": true }",
            CreatedAt = DateTime.UtcNow
        };

        var entitiesUnordered = new List<BlockchainData> { older, newer };

        var dtoOlder = new BlockchainDataDto
            (BlockchainBtc, older.RawJson, older.CreatedAt);

        var dtoNewer = new BlockchainDataDto(BlockchainBtc, newer.RawJson, newer.CreatedAt);

        var dtosUnordered = new List<BlockchainDataDto> { dtoOlder, dtoNewer };

        repository.GetHistoryAsync(BlockchainBtc, Arg.Any<CancellationToken>())
                  .Returns(entitiesUnordered);

        mapper.Map<IReadOnlyList<BlockchainDataDto>>(entitiesUnordered)
              .Returns(dtosUnordered);

        var query = new GetBlockchainHistoryQuery(BlockchainBtc);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(dtosUnordered);
    }
}