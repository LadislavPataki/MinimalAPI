using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiEndpoints.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection 
        AddEndpoints(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddEndpointsFromAssemblies(assemblies);
        services.AddValidatorsFromAssemblies(assemblies);
            
        return services;
    }

    private static IServiceCollection AddEndpointsFromAssemblies(
        this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var endpoints = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(EndpointBase).IsAssignableFrom(x)
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(endpoint);
        }

        return services;
    }
}