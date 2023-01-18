global using TodoApi.Common;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Common.DateTime;
using TodoApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// add api versioning
services
    .AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

services.AddEndpointsApiExplorer();

// add swagger
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

// add services for problem details
services.AddProblemDetails();

// add services for authentication and authorization
services.AddAuthentication()
    .AddJwtBearer();

services.AddAuthorization(options =>
    options.AddPolicy("todo:read-write", policyBuilder => policyBuilder
        .RequireAuthenticatedUser()
        .RequireRole("admin")
        .RequireClaim("scope", "todo:readwrite")
    )
);

// services.AddAuthorizationBuilder()
//     .AddPolicy("admin_todos", policy => 
//         policy
//             .RequireRole("admin")
//             .RequireScope("todos_api"));

// add common services
services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

// add modules
services.AddModules(configuration);

// add endpoints
services.AddEndpoints();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// it's not necessary to invoke UseAuthentication or UseAuthorization to register the middlewares
// because WebApplication does this automatically after AddAuthentication or AddAuthorization are called
app.UseAuthentication();
app.UseAuthorization();

// map endpoints
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach (var description in descriptions.Reverse())
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.Run();