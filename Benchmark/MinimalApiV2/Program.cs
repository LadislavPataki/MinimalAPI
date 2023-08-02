using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinimalApiReprV2.Features.PostBenchmark;
using MinimalApiV2.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

var services = builder.Services;

//services.AddEndpoints();

services.AddScoped(typeof(PostBenchmarkEndpoint));

//services.AddValidatorsFromAssemblyContaining(typeof(Program));
services.AddSingleton<IValidator<PostBenchmarkRequest>, PostBenchmarkRequestValidator>();
// services.AddSingleton<IValidator<LoadTestRequest>, LoadTestRequestValidator>();

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

services.AddAuthorization();

var app = builder.Build();

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

app.UseAuthorization();

//app.MapEndpoints();

var versionedEndpointRouteBuilder = app.NewVersionedApi();
var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup("/api/v{version:apiVersion}");


versionedApiRouteGroupBuilder.MapPost("/benchmark/ok/{id}", async (
        [NotNull, FromServices] PostBenchmarkEndpoint endpoint, 
        [NotNull, AsParameters] PostBenchmarkRequest request, 
        [FromServices] IValidator<PostBenchmarkRequest>? validator,
        HttpContext httpContext, 
        CancellationToken cancellationToken) =>
    {
        //var validator = httpContext.RequestServices.GetService<IValidator<PostBenchmarkRequest>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(
                    validationResult.ToDictionary(), 
                    statusCode: StatusCodes.Status422UnprocessableEntity);
            }
        }
        
        return Results.Ok(new PostBenchmarkResponse()
        {
            Id = request.Id,
            Name = request.PostBenchmarkRequestBody.FirstName + " " + request.PostBenchmarkRequestBody.LastName,
            Age = request.PostBenchmarkRequestBody.Age,
            PhoneNumber = request.PostBenchmarkRequestBody.PhoneNumbers?.FirstOrDefault()
        });
    })
    .MapToApiVersion(1)
    .RequireAuthorization()
    .AllowAnonymous();

app.Run();

namespace MinimalApiV2
{
    public partial class Program { }
}