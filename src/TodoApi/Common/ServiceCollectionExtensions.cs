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

public static class WebApplicationExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        var scope = app.Services.CreateScope();

        var versionedEndpointRouteBuilder = app.NewVersionedApi();
        var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup("/api/v{version:apiVersion}");

        var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.AddEndpoint(versionedApiRouteGroupBuilder);
        }
    }
}