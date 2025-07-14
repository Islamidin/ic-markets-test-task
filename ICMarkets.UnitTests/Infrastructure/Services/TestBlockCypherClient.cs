using System.Net;
using FluentAssertions;
using ICMarkets.Domain.Enums;
using ICMarkets.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ICMarkets.UnitTests.Infrastructure.Services;

[TestFixture]
public class TestBlockCypherClient
{
    private BlockCypherClient client = null!;
    private FakeHttpHandler handler = null!;
    private HttpClient httpClient = null!;
    private ILogger<BlockCypherClient> logger = null!;

    [SetUp]
    public void SetUp()
    {
        handler = new();
        httpClient = new(handler);
        logger = Substitute.For<ILogger<BlockCypherClient>>();
        client = new(httpClient, logger);
    }

    [Test]
    public async Task Should_Return_ValidResponse()
    {
        handler.Response = new(HttpStatusCode.OK)
        {
            Content = new StringContent("{ \"height\": 12345 }")
        };

        var result = await client.GetBlockchainInfoAsync(BlockchainType.Btc);
        result.Should().Contain("12345");
    }

    [Test]
    public async Task Should_Throw_On_Invalid_Blockchain()
    {
        Func<Task> act = async () => await client.GetBlockchainInfoAsync((BlockchainType) 999);

        await act.Should()
                 .ThrowAsync<ArgumentException>()
                 .WithMessage("*Unsupported blockchain*");
    }

    [Test]
    public async Task Should_Throw_On_FailedStatusCode()
    {
        handler.Response = new(HttpStatusCode.BadRequest);

        Func<Task> act = async () => await client.GetBlockchainInfoAsync(BlockchainType.Btc);

        await act.Should()
                 .ThrowAsync<HttpRequestException>();
    }

    private class FakeHttpHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; } = new(HttpStatusCode.OK);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(Response);
    }
}