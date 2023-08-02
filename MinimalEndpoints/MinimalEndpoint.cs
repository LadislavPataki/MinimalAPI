using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Eurowag.Common.AspNetCore.MinimalEndpoints;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public static class MinimalEndpoint
{
    public static class WithRequest<TRequest>
        where TRequest : class
    {
        public abstract class WithResponse<TResponse> : MinimalEndpointBase
            where TResponse : class
        {
            public async Task<IResult> HandleAsync(TRequest request,
                IValidator<TRequest>? validator,
                HttpContext httpContext,
                CancellationToken cancellationToken)
            {
                HttpContext = httpContext;

                if (validator is null)
                    return await HandleAsync(request, cancellationToken);

                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    // rework, extract to method maybe introduce custom response
                    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#customizing-responses
                    return TypedResults.Json(
                        new
                        {
                            Errors = validationResult.Errors
                                .Select(x => x.ErrorCode)
                                .Distinct()
                        },
                        contentType: ProblemDetailsDefaults.MediaType.Json,
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                return await HandleAsync(request, cancellationToken);
            }

            protected abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
        }

        public abstract class WithoutResponse : MinimalEndpointBase
        {
            public async Task<IResult> HandleAsync(TRequest request,
                IValidator<TRequest>? validator,
                HttpContext httpContext,
                CancellationToken cancellationToken)
            {
                HttpContext = httpContext;

                if (validator is null)
                    return await HandleAsync(request, cancellationToken);

                var validationResult = await validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    // rework, extract to method maybe introduce custom response
                    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#customizing-responses
                    return TypedResults.Json(
                        new
                        {
                            Errors = validationResult.Errors
                                .Select(x => x.ErrorCode)
                                .Distinct()
                        },
                        contentType: ProblemDetailsDefaults.MediaType.Json,
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                return await HandleAsync(request, cancellationToken);
            }

            protected abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
        }
    }

    public static class WithoutRequest
    {
        public abstract class WithResponse<TResponse> : MinimalEndpointBase
            where TResponse : class
        {
            public async Task<IResult> HandleAsync(
                HttpContext httpContext,
                CancellationToken cancellationToken)
            {
                HttpContext = httpContext;

                return await HandleAsync(cancellationToken);
            }

            protected abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);
        }

        public abstract class WithoutResponse : MinimalEndpointBase
        {
            public async Task<IResult> HandleAsync(
                HttpContext httpContext,
                CancellationToken cancellationToken)
            {
                HttpContext = httpContext;

                return await HandleAsync(cancellationToken);
            }

            protected abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);
        }
    }
}