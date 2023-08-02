using System.Diagnostics.CodeAnalysis;

namespace MinimalApiEndpoints.Extensions;

public class MapEndpointsOptions
{
    [StringSyntax("Route")] public string Prefix { get; set; } = "/api/v{version:apiVersion}";
}