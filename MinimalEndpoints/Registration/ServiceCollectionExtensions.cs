using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Eurowag.Common.AspNetCore.MinimalEndpoints.Registration;

[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMinimalEndpoints(
        this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddMinimalEndpointsFromAssemblies(assemblies);
        services.AddValidatorsFromAssemblies(assemblies);

        return services;
    }

    private static IServiceCollection AddMinimalEndpointsFromAssemblies(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        var endpoints = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(MinimalEndpointBase).IsAssignableFrom(x)
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(endpoint);
        }

        return services;
    }
}