using FluentAssertions;
using ICMarkets.Domain.Entities;
using ICMarkets.Domain.Enums;
using ICMarkets.Persistence.Context;
using ICMarkets.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.UnitTests.Persistence.Repositories;

[TestFixture]
public class TestBlockchainRepository
{
    private AppDbContext context = null!;
    private BlockchainRepository repository = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;

        context = new(options);
        repository = new(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task AddAsync_ShouldPersistData()
    {
        var entity = new BlockchainData
        {
            Id = Guid.NewGuid(),
            Blockchain = BlockchainType.Btc,
            RawJson = "{}",
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(entity, CancellationToken.None);

        var saved = await context.BlockchainData.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved.Blockchain.Should().Be(BlockchainType.Btc);
    }

    [Test]
    public async Task GetHistoryAsync_ShouldReturnDescendingOrder()
    {
        var oldData = new BlockchainData
        {
            Id = Guid.NewGuid(),
            Blockchain = BlockchainType.Btc,
            RawJson = "old",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        var newData = new BlockchainData
        {
            Id = Guid.NewGuid(),
            Blockchain = BlockchainType.Btc,
            RawJson = "new",
            CreatedAt = DateTime.UtcNow
        };

        await context.BlockchainData.AddRangeAsync(oldData, newData);
        await context.SaveChangesAsync();

        var result = await repository.GetHistoryAsync(BlockchainType.Btc, CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].RawJson.Should().Be("new");
        result[1].RawJson.Should().Be("old");
    }
}