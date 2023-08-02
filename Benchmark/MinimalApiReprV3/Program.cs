using FluentValidation;
using MinimalApiEndpoints.Extensions;
using MinimalApiReprV2.Features.PostBenchmark;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services
    .AddAuthorization();

var services = builder.Services;
services.AddEndpoints();
//services.AddSingleton<IValidator<PostBenchmarkRequest>, PostBenchmarkRequestValidator>();

var app = builder.Build();
app.UseAuthorization();

app.MapEndpointsWithoutVersioning();

app.Run();

namespace MinimalApiReprV3
{
    public partial class Program { }
}