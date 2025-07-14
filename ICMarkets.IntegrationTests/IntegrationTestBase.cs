using ICMarkets.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.IntegrationTests;

public class IntegrationTestBase
{
    private ServiceProvider? provider;
    protected WebApplicationFactory<Program> Factory { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetupBase()
    {
        provider = new ServiceCollection()
                   .AddEntityFrameworkInMemoryDatabase()
                   .BuildServiceProvider();

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var dbContextDescriptors = services
                                               .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                                                           d.ServiceType == typeof(AppDbContext))
                                               .ToList();

                    foreach (var descriptor in dbContextDescriptors)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                        options.UseInternalServiceProvider(provider);
                    });
                });
            });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownBase()
    {
        await Factory.DisposeAsync();
        if (provider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            provider?.DisposeAsync();
        }
    }
}