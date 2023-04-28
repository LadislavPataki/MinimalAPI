global using TodoApi.Common;
using Microsoft.AspNetCore.Diagnostics;
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
services.AddProblemDetails(options =>
{
    // this work only when DefaultProblemDetailsWriter is used
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
});

//services.AddProblemDetails();



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

// add output cache services
services.AddOutputCache(options =>
{
    //options.AddBasePolicy();
    // options.AddPolicy("nocache", policyBuilder => policyBuilder.NoCache());
    // options.AddPolicy("cache5000ms", policyBuilder => policyBuilder.Expire(TimeSpan.FromMilliseconds(5000)));

    options.AddBasePolicy(policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(10)));
    options.AddPolicy("Expire20", policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(20)));
    options.AddPolicy("Expire30", policyBuilder => policyBuilder.Expire(TimeSpan.FromSeconds(30)));
    
    
});

// add common services
services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

// add modules
services.AddModules(configuration);

// add endpoints
services.AddEndpoints();

var app = builder.Build();


//app.UseExceptionHandler();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";

        if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (exceptionHandlerFeature?.Error is { } exception)
            {
                var statusCode = exception switch
                {
                    BadHttpRequestException badHttpRequestException =>
                    (
                        badHttpRequestException.StatusCode
                    ),
                    _ =>
                    (
                        StatusCodes.Status500InternalServerError
                    )
                };
                
                context.Response.StatusCode = statusCode;
                
                // It calls DefaultProblemDetailsWriter.WriteAsync which applies defaults
                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context
                });
            }
        }
    });
});

app.UseStatusCodePages();

// it's not necessary to invoke UseAuthentication or UseAuthorization to register the middlewares
// because WebApplication does this automatically after AddAuthentication or AddAuthorization are called
// app.UseAuthentication();
// app.UseAuthorization();

// map endpoints
app.MapEndpoints();

// output caching
app.UseOutputCache();

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