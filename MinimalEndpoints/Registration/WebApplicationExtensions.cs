using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Eurowag.Common.AspNetCore.MinimalEndpoints.Registration;

public static class WebApplicationExtensions
{
    public static void MapMinimalEndpoints(
        this WebApplication app,
        Action<MinimalEndpointsConfiguration>? options = null)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        app.MapMinimalEndpointsFromAssemblies(assemblies, options);
    }

    private static void MapMinimalEndpointsFromAssemblies(
        this WebApplication app,
        IEnumerable<Assembly> assemblies,
        Action<MinimalEndpointsConfiguration>? options = null)
    {
        var mapEndpointsOptions = new MinimalEndpointsConfiguration();
        options?.Invoke(mapEndpointsOptions);

        var versionedEndpointRouteBuilder = app.NewVersionedApi();
        var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup(mapEndpointsOptions.Prefix);

        var endpointTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(MinimalEndpointBase).IsAssignableFrom(x)
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var endpointType in endpointTypes)
        {
            var scope = app.Services.CreateScope();
            var endpointInstance = scope.ServiceProvider.GetRequiredService(endpointType);

            var configureEndpointMethodInfo = endpointType.GetMethod(nameof(MinimalEndpointBase.ConfigureEndpoint));
            configureEndpointMethodInfo!.Invoke(
                endpointInstance,
                new object?[]
                {
                    new MinimalEndpointRouteHandlerBuilder(versionedApiRouteGroupBuilder, endpointType)
                });
        }
    }
}