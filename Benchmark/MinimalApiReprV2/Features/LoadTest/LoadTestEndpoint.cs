// using MinimalApiEndpoints;
// using Endpoint = MinimalApiEndpoints.Endpoint;
//
// namespace MinimalApiReprV2.Features.LoadTest;
//
// public class LoadTestEndpoint : Endpoint
//     .WithRequest<LoadTestRequest>
//     .WithResponse<LoadTestResponse>
// {
//     public override void ConfigureEndpoint(EndpointRouteHandlerBuilder builder)
//     {
//         builder
//             .MapGet("/load-test/ok/{id}")
//             .MapToApiVersion(1)
//             .RequireAuthorization()
//             .AllowAnonymous();
//     }
//
//     protected override async Task<IResult> HandleAsync(
//         LoadTestRequest request, CancellationToken cancellationToken)
//     {
//         var response = new LoadTestResponse()
//         {
//             Message = $"Hello {request.Id}"
//         };
//
//         return Results.Ok(response);
//     }
// }