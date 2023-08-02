using Microsoft.AspNetCore.Http;

namespace MinimalApiEndpoints;

public abstract class EndpointBase
{
    protected HttpContext HttpContext { get; set; }
    public abstract void ConfigureEndpoint(EndpointRouteHandlerBuilder builder);
}