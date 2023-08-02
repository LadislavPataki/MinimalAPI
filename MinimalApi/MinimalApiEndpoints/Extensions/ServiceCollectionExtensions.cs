using System.Reflection;

namespace MinimalApi.MinimalApiEndpoints.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection 
        AddEndpoints(this IServiceCollection services)
    {
        var endpoints = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(EndpointBase).IsAssignableFrom(x) 
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(endpoint);
        }

        return services;
    }
}