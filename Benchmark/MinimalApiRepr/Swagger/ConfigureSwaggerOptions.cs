using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApiRepr.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme.",
        });

        // options.AddSecurityDefinition("Oauth2", new OpenApiSecurityScheme()
        // {
        //     Name = "Authorization",
        //     Type = SecuritySchemeType.OAuth2,
        //     Flows = new OpenApiOAuthFlows()
        //     {
        //         AuthorizationCode = new OpenApiOAuthFlow()
        //         {
        //             AuthorizationUrl = new Uri("https://example.com/oauth/authorize", UriKind.Absolute),
        //             TokenUrl = new Uri("https://example.com/oauth/token", UriKind.Absolute),
        //             Scopes = new Dictionary<string, string>
        //             {
        //                 { "todo:read-write", "Todo api read-write access" },
        //                 // { "writeAccess", "Access write operations" }
        //             }
        //         }
        //     },
        //     Description = "oath2"
        //     
        // });
        //
        // options.AddSecurityDefinition("OpenID", new OpenApiSecurityScheme()
        // {
        //     Type = SecuritySchemeType.OpenIdConnect,
        //     OpenIdConnectUrl = new Uri("https://example.com/.well-known/openid-configuration", UriKind.Absolute)
        // });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "BearerAuth"
                    }
                },
                Array.Empty<string>()
            },
            // {
            //     new OpenApiSecurityScheme
            //     {
            //         Reference = new OpenApiReference
            //         {
            //             Type = ReferenceType.SecurityScheme,
            //             Id = "Oauth2"
            //         }
            //     },
            //     new List<string>() { "todo:read-write" }
            // }
        });
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder("An example application with OpenAPI, Swashbuckle, and API versioning.");
        var info = new OpenApiInfo()
        {
            Title = "Example API",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact() { Name = "Bill Mei", Email = "bill.mei@somewhere.com" },
            License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        if (description.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                foreach (var link in policy.Links)
                {
                    if (link.Type != "text/html")
                        continue;
                        
                    text.AppendLine();

                    if (link.Title.HasValue)
                    {
                        text.Append(link.Title.Value).Append(": ");
                    }

                    text.Append(link.LinkTarget.OriginalString);
                }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}