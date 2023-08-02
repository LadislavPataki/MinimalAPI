using System.Text;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MinimalApi.Swagger;

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
        
        options.SchemaFilter<FluentValidationSchemaFilter>();

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

public class FluentValidationSchemaFilter : ISchemaFilter
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationSchemaFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var genericType = typeof(IValidator<>).MakeGenericType(context.Type);
        if (_serviceProvider.GetService(genericType) is not IValidator validator) 
            return;

        var validatorDescriptor = validator.CreateDescriptor();

        foreach (var property in schema.Properties)
        {
            
        }
    }
}


// public class AddFluentValidationRules : ISchemaFilter
// {
//     private readonly IValidatorFactory _factory;
//
//     /// <summary>
//     ///     Default constructor with DI
//     /// </summary>
//     /// <param name="factory"></param>
//     public AddFluentValidationRules(IValidatorFactory factory)
//     {
//         _factory = factory;
//     }
//
//     /// <summary>
//     /// </summary>
//     /// <param name="model"></param>
//     /// <param name="context"></param>
//     public void Apply(Schema model, SchemaFilterContext context)
//     {
//
//         // use IoC or FluentValidatorFactory to get AbstractValidator<T> instance
//         var validator = _factory.GetValidator(context.SystemType);
//         if (validator == null) return;
//         if (model.Required == null)
//             model.Required = new List<string>();
//
//         var validatorDescriptor = validator.CreateDescriptor();
//         foreach (var key in model.Properties.Keys)
//         {
//             foreach (var propertyValidator in validatorDescriptor
//                 .GetValidatorsForMember(ToPascalCase(key)))
//             {
//                 if (propertyValidator is NotNullValidator<,> || propertyValidator is NotEmptyValidator)
//                     model.Required.Add(key);
//
//                 if (propertyValidator is LengthValidator lengthValidator)
//                 {
//                     if (lengthValidator.Max > 0)
//                         model.Properties[key].MaxLength = lengthValidator.Max;
//
//                     model.Properties[key].MinLength = lengthValidator.Min;
//                 }
//
//                 if (propertyValidator is RegularExpressionValidator expressionValidator)
//                     model.Properties[key].Pattern = expressionValidator.Expression;
//
//                 // Add more validation properties here;
//             }
//         }
//     }
//
//     /// <summary>
//     ///     To convert case as swagger may be using lower camel case
//     /// </summary>
//     /// <param name="inputString"></param>
//     /// <returns></returns>
//     private static string ToPascalCase(string inputString)
//     {
//         // If there are 0 or 1 characters, just return the string.
//         if (inputString == null) return null;
//         if (inputString.Length < 2) return inputString.ToUpper();
//         return inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
//     }
// }