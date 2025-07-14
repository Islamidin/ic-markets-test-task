using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using ICMarkets.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(assembly); });

        services.AddAutoMapper(assembly);

        services.AddValidatorsFromAssembly(assembly);
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}