using Eurowag.Common.AspNetCore.MinimalEndpoints.Registration;
using FluentValidation;
using Microsoft.Extensions.Options;
using MinimalApi.Swagger;
// using MinimalApiEndpoints.Extensions;
using MinimalApiReprV2.Features.LoadTest;
using MinimalApiReprV2.Features.PostBenchmark;
using MinimalApiReprV2.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();

var services = builder.Services;

services.AddMinimalEndpoints();
// services.AddEndpoints();

// services.AddValidatorsFromAssemblyContaining(typeof(Program));
// services.AddSingleton<IValidator<PostBenchmarkRequest>, PostBenchmarkRequestValidator>();
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

app.MapMinimalEndpoints();
// app.MapEndpoints();

app.MapGet("/testpath", () => Results.Ok());

app.Run();

namespace MinimalApiReprV2
{
    public partial class Program { }
}