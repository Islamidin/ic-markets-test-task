using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ICMarkets.Application.DTO;
using ICMarkets.Domain.Enums;

namespace ICMarkets.IntegrationTests.Tests;

[TestFixture]
public class BlockchainControllerTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Client = Factory.CreateClient();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Client.Dispose();
    }

    [Test]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await Client.GetAsync("/health");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Fetch_Post_ReturnsOk()
    {
        const BlockchainType blockchainType = BlockchainType.Btc;

        var response = await Client.PostAsync($"/api/blockchain/{blockchainType}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var content = await response.Content.ReadAsStringAsync();
        Assert.That(content, Is.EqualTo("Data fetched successfully."));
    }

    [Test]
    public async Task History_Get_ReturnsBlockchainDataDtoList()
    {
        const BlockchainType blockchainType = BlockchainType.Btc;

        var postResponse = await Client.PostAsync($"/api/blockchain/{blockchainType}", null);
        Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var response = await Client.GetAsync($"/api/blockchain/{blockchainType}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var data = await response.Content.ReadFromJsonAsync<IReadOnlyList<BlockchainDataDto>>(JsonOptions);
        Assert.That(data, Is.Not.Null, "Response data should not be null");
        Assert.That(data, Is.InstanceOf<IReadOnlyList<BlockchainDataDto>>(), "Response data should be IReadOnlyList<BlockchainDataDto>");
        Assert.That(data!.Count, Is.GreaterThan(0), "Response data should not be empty");

        foreach (var item in data)
        {
            Assert.That(Enum.IsDefined(typeof(BlockchainType), item.Blockchain), Is.True, $"Invalid BlockchainType value: {item.Blockchain}");
            Assert.That(item.RawJson, Is.Not.Null.And.Not.Empty, "RawJson should not be null or empty");
            Assert.That(item.CreatedAt, Is.Not.EqualTo(default(DateTime)), "CreatedAt should have a valid value");
        }
    }

    private HttpClient Client { get; set; } = null!;
}