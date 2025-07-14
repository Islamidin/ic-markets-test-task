namespace ICMarkets.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "ICMarkets API",
                Version = "v1",
                Description = "Stores and retrieves blockchain data from BlockCypher"
            });
        });

        return services;
    }
}