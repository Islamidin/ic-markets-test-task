using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ICMarkets.Application.DependencyInjection;

public static class AutoMapperServiceCollectionExtensions
{
    private static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression>? configAction = null)
    {
        services.AddLogging();

        services.AddSingleton(provider =>
        {
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var configExpr = new MapperConfigurationExpression();
            configAction?.Invoke(configExpr);
            return new MapperConfiguration(configExpr, loggerFactory);
        });

        services.AddSingleton<IMapper>(provider =>
        {
            var config = provider.GetRequiredService<MapperConfiguration>();
            return config.CreateMapper();
        });

        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.AddAutoMapper(cfg =>
        {
            foreach (var assembly in assemblies)
            {
                cfg.AddMaps(assembly);
            }
        });
    }
}