using MinimalApiEndpoints;
using MinimalApiReprV2.Features.PostBenchmark;
using Endpoint = MinimalApiEndpoints.Endpoint;

namespace MinimalApiReprV3.PostBenchmark;

public class PostBenchmarkEndpoint : Endpoint
    .WithRequest<PostBenchmarkRequest>
    .WithResponse<PostBenchmarkResponse>
{
    public override void ConfigureEndpoint(EndpointRouteHandlerBuilder builder)
    {
        builder
            .MapPost("/benchmark/ok/{id}")
            // .MapToApiVersion(1)
            .RequireAuthorization()
            .AllowAnonymous();
    }

    protected override async Task<IResult> HandleAsync(
        PostBenchmarkRequest request, CancellationToken cancellationToken)
    {
        return Results.Ok(new PostBenchmarkResponse()
        {
            Id = request.Id,
            Name = request.PostBenchmarkRequestBody.FirstName + " " + request.PostBenchmarkRequestBody.LastName,
            Age = request.PostBenchmarkRequestBody.Age,
            PhoneNumber = request.PostBenchmarkRequestBody.PhoneNumbers?.FirstOrDefault()
        });
    }
}