
namespace MinimalApiReprV2.Features.PostBenchmark;

public class PostBenchmarkEndpoint 
{
    protected async Task<IResult> HandleAsync(
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