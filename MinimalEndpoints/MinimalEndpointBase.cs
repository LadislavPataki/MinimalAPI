using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Eurowag.Common.AspNetCore.MinimalEndpoints;

public abstract class MinimalEndpointBase
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    protected HttpContext HttpContext { get; set; } = null!;
    public abstract void ConfigureEndpoint(MinimalEndpointRouteHandlerBuilder builder);
}