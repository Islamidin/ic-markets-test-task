using FluentAssertions;
using ICMarkets.Application.Commands.FetchBlockchainData;
using ICMarkets.Application.Interfaces;
using ICMarkets.Domain.Entities;
using ICMarkets.Domain.Enums;
using ICMarkets.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ICMarkets.UnitTests.Application.Handlers;

[TestFixture]
public class TestFetchBlockchainDataHandler
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private IBlockCypherClient client;
    private FetchBlockchainDataHandler handler;
    private IBlockchainRepository repository;

    [SetUp]
    public void Setup()
    {
        client = Substitute.For<IBlockCypherClient>();
        repository = Substitute.For<IBlockchainRepository>();
        handler = new(client, repository);
    }

    [Test]
    public async Task Handle_ValidRequest_AddsDataToRepository()
    {
        const BlockchainType blockchain = BlockchainType.Btc;
        const string json = "{ \"test\": 123 }";

        client.GetBlockchainInfoAsync(blockchain, cancellationToken).Returns(json);

        await handler.Handle(new(blockchain), cancellationToken);

        await repository.Received(1).AddAsync(Arg.Is<BlockchainData>(data =>
                                                                         data.Blockchain == blockchain &&
                                                                         data.RawJson == json &&
                                                                         data.CreatedAt != default
                                              ), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ClientThrowsException_ShouldPropagate()
    {
        const BlockchainType blockchain = BlockchainType.Eth;
        client.GetBlockchainInfoAsync(blockchain, cancellationToken).Throws(new Exception("API failed"));

        Func<Task> act = async () =>
            await handler.Handle(new(blockchain), cancellationToken);

        await act.Should().ThrowAsync<Exception>().WithMessage("API failed");
        await repository.DidNotReceive().AddAsync(Arg.Any<BlockchainData>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_RepositoryThrowsException_ShouldPropagate()
    {
        const BlockchainType blockchain = BlockchainType.Ltc;
        client.GetBlockchainInfoAsync(blockchain, cancellationToken).Returns("{ \"height\": 9999 }");
        repository.AddAsync(Arg.Any<BlockchainData>(), Arg.Any<CancellationToken>()).Throws(new Exception("DB error"));

        Func<Task> act = async () =>
            await handler.Handle(new(blockchain), cancellationToken);

        await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
    }

    [Test]
    public async Task Handle_ShouldSet_CreatedAt_ToRecentTime()
    {
        const BlockchainType blockchain = BlockchainType.Dash;
        const string json = "{}";
        client.GetBlockchainInfoAsync(blockchain, cancellationToken).Returns(json);

        BlockchainData? savedData = null;
        await repository.AddAsync(Arg.Do<BlockchainData>(data => savedData = data), Arg.Any<CancellationToken>());

        await handler.Handle(new(blockchain), cancellationToken);

        savedData.Should().NotBeNull();
        savedData!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Test]
    public async Task Handle_WhenJsonIsEmpty_ShouldStillAddToRepository()
    {
        const BlockchainType blockchain = BlockchainType.Eth;
        client.GetBlockchainInfoAsync(blockchain, cancellationToken).Returns("");

        await handler.Handle(new(blockchain), cancellationToken);

        await repository.Received(1).AddAsync(Arg.Is<BlockchainData>(data =>
                                                                         data.RawJson == "" &&
                                                                         data.Blockchain == blockchain
                                              ), Arg.Any<CancellationToken>());
    }
}