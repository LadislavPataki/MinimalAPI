using System.Diagnostics.CodeAnalysis;

namespace Eurowag.Common.AspNetCore.MinimalEndpoints.Registration;

public class MinimalEndpointsConfiguration
{
    [StringSyntax("Route")] public string Prefix { get; set; } = "/v{version:apiVersion}";
}