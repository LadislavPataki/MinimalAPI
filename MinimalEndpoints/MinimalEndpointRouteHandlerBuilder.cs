using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Eurowag.Common.AspNetCore.MinimalEndpoints.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Eurowag.Common.AspNetCore.MinimalEndpoints;

public class MinimalEndpointRouteHandlerBuilder 
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;
    private readonly Type _endpointType;
   
    public MinimalEndpointRouteHandlerBuilder(
        IEndpointRouteBuilder endpointRouteBuilder,
        Type endpointType)
    {
        ArgumentNullException.ThrowIfNull(endpointRouteBuilder);
        ArgumentNullException.ThrowIfNull(endpointType);

        if (!typeof(MinimalEndpointBase).IsAssignableFrom(endpointType))
            throw new ArgumentException(
                $"The parameter must be a derived type of {nameof(MinimalEndpointBase)}",
                nameof(endpointType));

        _endpointRouteBuilder = endpointRouteBuilder;
        _endpointType = endpointType;
    }

    public RouteHandlerBuilder MapGet([StringSyntax("Route")] string pattern)
    {
        var routeHandlerBuilder = _endpointRouteBuilder.MapGet(pattern, GetHandler());

        return routeHandlerBuilder;
    }

    public RouteHandlerBuilder MapPost([StringSyntax("Route")] string pattern)
    {
        var routeHandlerBuilder = _endpointRouteBuilder.MapPost(pattern, GetHandler());

        return routeHandlerBuilder;
    }
    
    public RouteHandlerBuilder MapPut([StringSyntax("Route")] string pattern)
    {
        var routeHandlerBuilder = _endpointRouteBuilder.MapPut(pattern, GetHandler());

        return routeHandlerBuilder;
    }

    public RouteHandlerBuilder MapDelete([StringSyntax("Route")] string pattern)
    {
        var routeHandlerBuilder = _endpointRouteBuilder.MapDelete(pattern, GetHandler());

        return routeHandlerBuilder;
    }

    private Delegate GetHandler()
    {
        if (_endpointType.IsSubclassOfOpenGeneric(typeof(MinimalEndpoint.WithRequest<>.WithResponse<>)))
            return GetHandlerOfEndpointWithRequestAndWithResponse();

        if (_endpointType.IsSubclassOfOpenGeneric(typeof(MinimalEndpoint.WithRequest<>.WithoutResponse)))
            return GetHandlerOfEndpointWithRequestAndWithoutResponse();

        if (_endpointType.IsSubclassOfOpenGeneric(typeof(MinimalEndpoint.WithoutRequest.WithResponse<>)))
            return GetHandlerOfEndpointWithoutRequestAndWithResponse();

        if (_endpointType.IsSubclassOfOpenGeneric(typeof(MinimalEndpoint.WithoutRequest.WithResponse<>)))
            return GetHandlerOfEndpointWithoutRequestAndWithoutResponse();

        throw new InvalidOperationException();
    }

    private Delegate GetHandlerOfEndpointWithoutRequestAndWithoutResponse()
    {
        var type = typeof(MinimalEndpointRouteHandlerBuilder);
        var handleRequestMethodInfo =
            type.GetMethod(nameof(HandleRequestForEndpointWithoutRequestAndWithoutResponse));
        var handleRequestGenericMethodInfo =
            handleRequestMethodInfo!.MakeGenericMethod(_endpointType);

        var handleRequestDelegateType = Expression.GetDelegateType(
            _endpointType,
            typeof(HttpContext),
            typeof(CancellationToken),
            typeof(Task<IResult>));

        var handleRequestDelegate =
            Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);

        return handleRequestDelegate;
    }

    private Delegate GetHandlerOfEndpointWithoutRequestAndWithResponse()
    {
        var endpointBaseType = _endpointType.BaseType;
        var endpointBaseGenericArguments = endpointBaseType!.GetGenericArguments();

        const int responseTypeIndex = 0;

        var responseType = endpointBaseGenericArguments[responseTypeIndex];

        var builderType = typeof(MinimalEndpointRouteHandlerBuilder);
        var handleRequestMethodInfo =
            builderType.GetMethod(nameof(HandleRequestForEndpointWithoutRequestAndWithResponse));
        var handleRequestGenericMethodInfo =
            handleRequestMethodInfo!.MakeGenericMethod(_endpointType, responseType);

        var handleRequestDelegateType = Expression.GetDelegateType(
            _endpointType,
            typeof(HttpContext),
            typeof(CancellationToken),
            typeof(Task<IResult>));

        var handleRequestDelegate =
            Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);

        return handleRequestDelegate;
    }

    private Delegate GetHandlerOfEndpointWithRequestAndWithoutResponse()
    {
        var endpointBaseType = _endpointType.BaseType;
        var endpointBaseGenericArguments = endpointBaseType!.GetGenericArguments();

        const int requestTypeIndex = 0;

        var requestType = endpointBaseGenericArguments[requestTypeIndex];

        var type = typeof(MinimalEndpointRouteHandlerBuilder);
        var handleRequestMethodInfo =
            type.GetMethod(nameof(HandleRequestForEndpointWithRequestAndWithoutResponse));
        var handleRequestGenericMethodInfo =
            handleRequestMethodInfo!.MakeGenericMethod(_endpointType, requestType);
        
        var validatorType = typeof(IValidator<>);
        var genericValidatorType = validatorType.MakeGenericType(requestType);

        var handleRequestDelegateType = Expression.GetDelegateType(
            _endpointType,
            requestType,
            genericValidatorType,
            typeof(HttpContext),
            typeof(CancellationToken),
            typeof(Task<IResult>));

        var handleRequestDelegate =
            Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);

        return handleRequestDelegate;
    }

    private Delegate GetHandlerOfEndpointWithRequestAndWithResponse()
    {
        var endpointBaseType = _endpointType.BaseType;
        var endpointBaseGenericArguments = endpointBaseType!.GetGenericArguments();

        const int requestTypeIndex = 0;
        const int responseTypeIndex = 1;

        var requestType = endpointBaseGenericArguments[requestTypeIndex];
        var responseType = endpointBaseGenericArguments[responseTypeIndex];

        var type = typeof(MinimalEndpointRouteHandlerBuilder);
        var handleRequestMethodInfo =
            type.GetMethod(nameof(HandleRequestForEndpointWithRequestAndWithResponse));
        var handleRequestGenericMethodInfo =
            handleRequestMethodInfo!.MakeGenericMethod(_endpointType, requestType, responseType);
        
        var validatorType = typeof(IValidator<>);
        var genericValidatorType = validatorType.MakeGenericType(requestType);

        var handleRequestDelegateType = Expression.GetDelegateType(
            _endpointType,
            requestType,
            genericValidatorType,
            typeof(HttpContext),
            typeof(CancellationToken),
            typeof(Task<IResult>));

        var handleRequestDelegate =
            Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);

        return handleRequestDelegate;
    }
    
    public static Task<IResult> HandleRequestForEndpointWithRequestAndWithResponse<TEndpoint, TRequest, TResponse>(
        [NotNull, FromServices] TEndpoint endpoint,
        [NotNull, AsParameters] TRequest request,
        [FromServices] IValidator<TRequest>? validator,
        HttpContext httpContext,
        CancellationToken cancellationToken) 
        where TEndpoint : MinimalEndpoint.WithRequest<TRequest>.WithResponse<TResponse>
        where TRequest : class
        where TResponse : class
    {
        return endpoint.HandleAsync(request, validator, httpContext, cancellationToken);
    }
    
    public static Task<IResult> HandleRequestForEndpointWithRequestAndWithoutResponse<TEndpoint, TRequest>(
        [NotNull, FromServices] TEndpoint endpoint,
        [NotNull, AsParameters] TRequest request,
        [FromServices] IValidator<TRequest>? validator,
        HttpContext httpContext,
        CancellationToken cancellationToken) 
        where TEndpoint : MinimalEndpoint.WithRequest<TRequest>.WithoutResponse
        where TRequest : class
    {
        return endpoint.HandleAsync(request, validator, httpContext, cancellationToken);
    }

    public static Task<IResult> HandleRequestForEndpointWithoutRequestAndWithResponse<TEndpoint, TResponse>(
        [NotNull, FromServices] TEndpoint endpoint,
        HttpContext httpContext,
        CancellationToken cancellationToken) 
        where TEndpoint : MinimalEndpoint.WithoutRequest.WithResponse<TResponse>
        where TResponse : class
    {
        return endpoint.HandleAsync(httpContext, cancellationToken);
    }

    public static Task<IResult> HandleRequestForEndpointWithoutRequestAndWithoutResponse<TEndpoint>(
        [NotNull, FromServices] TEndpoint endpoint,
        HttpContext httpContext,
        CancellationToken cancellationToken) 
        where TEndpoint : MinimalEndpoint.WithoutRequest.WithoutResponse
    {
        return endpoint.HandleAsync(httpContext, cancellationToken);
    }
}