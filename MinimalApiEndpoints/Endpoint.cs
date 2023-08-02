using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace MinimalApiEndpoints;

public static class Endpoint
{
    public static class WithRequest<TRequest> 
        where TRequest : class
    {
        public abstract class WithResponse<TResponse> : EndpointBase
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
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                return await HandleAsync(request, cancellationToken);
            }

            protected abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
        }

        public abstract class WithoutResponse : EndpointBase
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
                    return Results.ValidationProblem(
                        validationResult.ToDictionary(), 
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                }

                return await HandleAsync(request, cancellationToken);
            }

            protected abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
        }
    }

    public static class WithoutRequest
    {
        public abstract class WithResponse<TResponse> : EndpointBase
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

        public abstract class WithoutResponse : EndpointBase
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