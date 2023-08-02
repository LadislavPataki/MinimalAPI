using Microsoft.AspNetCore.Mvc;

namespace MinimalApiReprV2.Features.PostBenchmark;

public class PostBenchmarkRequest
{
    [FromRoute] public int Id { get; set; }
    [FromBody] public PostBenchmarkRequestBody PostBenchmarkRequestBody { get; set; }
}