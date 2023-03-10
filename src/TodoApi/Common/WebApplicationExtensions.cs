namespace TodoApi.Common;

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