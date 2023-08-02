using System.Diagnostics.CodeAnalysis;

namespace MinimalApi.MinimalApiEndpoints.Extensions;

public class MapEndpointsOptions
{
    [StringSyntax("Route")] public string Prefix { get; set; } = "/api/v{version:apiVersion}";
}