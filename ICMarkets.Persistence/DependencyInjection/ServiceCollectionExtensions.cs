using ICMarkets.Domain.Interfaces;
using ICMarkets.Persistence.Context;
using ICMarkets.Persistence.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ICMarkets.Persistence.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        if (env.IsEnvironment("Testing"))
        {
            services.AddDbContext<AppDbContext>(options =>
                                                    options.UseInMemoryDatabase("TestDb"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                                                    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }

        services.AddScoped<IBlockchainRepository, BlockchainRepository>();

        return services;
    }
}