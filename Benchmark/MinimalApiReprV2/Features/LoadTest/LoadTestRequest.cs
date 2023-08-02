using Microsoft.AspNetCore.Mvc;

namespace MinimalApiReprV2.Features.LoadTest;

public class LoadTestRequest
{
    [FromRoute] public int Id { get; set; }
}