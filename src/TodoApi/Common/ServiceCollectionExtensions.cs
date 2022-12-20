using System.Reflection;

namespace TodoApi.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpoints = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) 
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var endpoint in endpoints)
        {
            services.AddScoped(typeof(IEndpoint), endpoint);
        }

        return services;
    }
}