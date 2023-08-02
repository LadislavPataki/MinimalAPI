using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApiEndpoints.Extensions;

namespace MinimalApiEndpoints;

public class EndpointRouteHandlerBuilder 
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;
    private readonly Type _endpointType;
   
    public EndpointRouteHandlerBuilder(
        IEndpointRouteBuilder endpointRouteBuilder,
        Type endpointType)
    {
        ArgumentNullException.ThrowIfNull(endpointRouteBuilder);
        ArgumentNullException.ThrowIfNull(endpointType);

        if (!typeof(EndpointBase).IsAssignableFrom(endpointType))
            throw new ArgumentException(
                $"The parameter must be a derived type of {nameof(EndpointBase)}",
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
        if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithRequest<>.WithResponse<>)))
            return GetHandlerOfEndpointWithRequestAndWithResponse();

        if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithRequest<>.WithoutResponse)))
            return GetHandlerOfEndpointWithRequestAndWithoutResponse();

        if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithoutRequest.WithResponse<>)))
            return GetHandlerOfEndpointWithoutRequestAndWithResponse();

        if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithoutRequest.WithResponse<>)))
            return GetHandlerOfEndpointWithoutRequestAndWithoutResponse();

        throw new InvalidOperationException();
    }

    private Delegate GetHandlerOfEndpointWithoutRequestAndWithoutResponse()
    {
        var type = typeof(EndpointRouteHandlerBuilder);
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

        var builderType = typeof(EndpointRouteHandlerBuilder);
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

        var type = typeof(EndpointRouteHandlerBuilder);
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

        var type = typeof(EndpointRouteHandlerBuilder);
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
        where TEndpoint : Endpoint.WithRequest<TRequest>.WithResponse<TResponse>
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
        where TEndpoint : Endpoint.WithRequest<TRequest>.WithoutResponse
        where TRequest : class
    {
        return endpoint.HandleAsync(request, validator, httpContext, cancellationToken);
    }

    public static Task<IResult> HandleRequestForEndpointWithoutRequestAndWithResponse<TEndpoint, TResponse>(
        [NotNull, FromServices] TEndpoint endpoint,
        HttpContext httpContext,
        CancellationToken cancellationToken) 
        where TEndpoint : Endpoint.WithoutRequest.WithResponse<TResponse>
        where TResponse : class
    {
        return endpoint.HandleAsync(httpContext, cancellationToken);
    }

    public static Task<IResult> HandleRequestForEndpointWithoutRequestAndWithoutResponse<TEndpoint>(
        [NotNull, FromServices] TEndpoint endpoint,
        HttpContext httpContext,
        CancellationToken cancellationToken) 
        where TEndpoint : Endpoint.WithoutRequest.WithoutResponse
    {
        return endpoint.HandleAsync(httpContext, cancellationToken);
    }
}




// public class EndpointRouteHandlerBuilder  // : IMyRouteBuilder//, IEndpointConventionBuilder
// {
//     // private readonly IVersionedEndpointRouteBuilder _versionedEndpointRouteBuilder;
//     private readonly IEndpointRouteBuilder _endpointRouteBuilder;
//     
//     private readonly Type _endpointType;
//
//     // public MyRouteHandlerBuilder(IEndpointRouteBuilder endpointRouteBuilder)
//     // {
//     //     _endpointRouteBuilder = endpointRouteBuilder;
//     // }
//
//     public EndpointRouteHandlerBuilder(
//         IEndpointRouteBuilder endpointRouteBuilder,
//         Type endpointType)
//     {
//         ArgumentNullException.ThrowIfNull(endpointRouteBuilder);
//         ArgumentNullException.ThrowIfNull(endpointType);
//
//         if (!typeof(EndpointBase).IsAssignableFrom(endpointType))
//             throw new ArgumentException(
//                 $"The parameter must be a derived type of {nameof(EndpointBase)}",
//                 nameof(endpointType));
//
//
//         _endpointRouteBuilder = endpointRouteBuilder;
//         _endpointType = endpointType;
//     }
//
//     public RouteHandlerBuilder MapGet([StringSyntax("Route")] string pattern)
//     {
//         var routeHandlerBuilder = _endpointRouteBuilder.MapGet(pattern, GetHandler());
//
//         return routeHandlerBuilder;
//     }
//
//     public RouteHandlerBuilder MapPost([StringSyntax("Route")] string pattern)
//     {
//         var routeHandlerBuilder = _endpointRouteBuilder.MapPost(pattern, GetHandler());
//
//         return routeHandlerBuilder;
//
//         // var routeHandlerBuilder = _endpointRouteBuilder.MapPost(pattern, apiEndpointHandlerDelegate);
//         // var routeHandlerBuilder = _endpointRouteBuilder.MapPost(pattern, apiEndpointHandlerDelegate);
//         //
//         // return routeHandlerBuilder;
//     }
//     public RouteHandlerBuilder MapPut([StringSyntax("Route")] string pattern)
//     {
//         var routeHandlerBuilder = _endpointRouteBuilder.MapPut(pattern, GetHandler());
//
//         return routeHandlerBuilder;
//     }
//
//     public RouteHandlerBuilder MapDelete([StringSyntax("Route")] string pattern)
//     {
//         var routeHandlerBuilder = _endpointRouteBuilder.MapDelete(pattern, GetHandler());
//
//         return routeHandlerBuilder;
//     }
//
//     private Delegate GetHandler()
//     {
//         // var endpointBaseType = _endpointType.BaseType.BaseType;
//         // var endpointBaseTypeGenericTypeDefinition = endpointBaseType!.GetGenericTypeDefinition();
//         //
//         // if (endpointBaseTypeGenericTypeDefinition == typeof(EndpointWithRequestAndWithResponse<,>))
//         //     return GetHandlerOfEndpointWithRequestAndWithResponse();
//         //
//         // if (endpointBaseTypeGenericTypeDefinition == typeof(EndpointWithRequestAndWithoutResponse<>))
//         //     return GetHandlerOfEndpointWithRequestAndWithoutResponse();
//         //
//         // if (endpointBaseTypeGenericTypeDefinition == typeof(EndpointWithoutRequestAndWithResponse<>))
//         //     return GetHandlerOfEndpointWithoutRequestAndWithResponse();
//         //
//         // if (endpointBaseTypeGenericTypeDefinition == typeof(EndpointWithoutRequestAndWithoutResponse))
//         //     return GetHandlerOfEndpointWithoutRequestAndWithoutResponse();
//         
//         
//         
//         if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithRequest<>.WithResponse<>)))
//             return GetHandlerOfEndpointWithRequestAndWithResponse();
//
//         if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithRequest<>.WithoutResponse)))
//             return GetHandlerOfEndpointWithRequestAndWithoutResponse();
//
//         if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithoutRequest.WithResponse<>)))
//             return GetHandlerOfEndpointWithoutRequestAndWithResponse();
//
//         if (_endpointType.IsSubclassOfOpenGeneric(typeof(Endpoint.WithoutRequest.WithResponse<>)))
//             return GetHandlerOfEndpointWithoutRequestAndWithoutResponse();
//         
//         
//         
//         
//         
//         
//         // if (_endpointType.IsSubclassOfOpenGeneric(typeof(EndpointWithRequestAndWithResponse<,>)))
//         //     return GetHandlerOfEndpointWithRequestAndWithResponse();
//         //
//         // if (_endpointType.IsSubclassOfOpenGeneric(typeof(EndpointWithRequestAndWithoutResponse<>)))
//         //     return GetHandlerOfEndpointWithRequestAndWithoutResponse();
//         //
//         // if (_endpointType.IsSubclassOfOpenGeneric(typeof(EndpointWithoutRequestAndWithResponse<>)))
//         //     return GetHandlerOfEndpointWithoutRequestAndWithResponse();
//         //
//         // if (_endpointType.IsSubclassOfOpenGeneric(typeof(EndpointWithoutRequestAndWithoutResponse)))
//         //     return GetHandlerOfEndpointWithoutRequestAndWithoutResponse();
//
//         throw new InvalidOperationException();
//
//
//
//         // var endpointBaseGenericArguments = endpointBaseType.GetGenericArguments();
//         //
//         // // var isEndpointWithRequestAndResponse = endpointBaseGenericArguments.Length == 2;
//         // // if (isEndpointWithRequestAndResponse)
//         // {
//         //     var requestType = endpointBaseGenericArguments[requestTypeIndex];
//         //     var responseType = endpointBaseGenericArguments[responseTypeIndex];
//         //
//         //     var type = typeof(EndpointRouteHandlerBuilder);
//         //     var handleRequestMethodInfo =
//         //         type.GetMethod(nameof(HandleRequest));
//         //     var handleRequestGenericMethodInfo =
//         //         handleRequestMethodInfo.MakeGenericMethod(_endpointType, requestType, responseType);
//         //
//         //     var handleRequestDelegateType = Expression.GetDelegateType(
//         //         _endpointType,
//         //         requestType,
//         //         typeof(CancellationToken),
//         //         typeof(HttpContext),
//         //         typeof(Task<IResult>));
//         //
//         //     handleRequestDelegate =
//         //         Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);
//         // }
//         //
//         //
//         // return handleRequestDelegate;
//     }
//
//     private Delegate GetHandlerOfEndpointWithoutRequestAndWithoutResponse()
//     {
//         var type = typeof(EndpointRouteHandlerBuilder);
//         var handleRequestMethodInfo =
//             type.GetMethod(nameof(HandleRequestForEndpointWithoutRequestAndWithoutResponse));
//         var handleRequestGenericMethodInfo =
//             handleRequestMethodInfo!.MakeGenericMethod(_endpointType);
//
//         var handleRequestDelegateType = Expression.GetDelegateType(
//             _endpointType,
//             typeof(HttpContext),
//             typeof(CancellationToken),
//             typeof(Task<IResult>));
//
//         var handleRequestDelegate =
//             Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);
//
//         return handleRequestDelegate;
//     }
//
//     private Delegate GetHandlerOfEndpointWithoutRequestAndWithResponse()
//     {
//         var endpointBaseType = _endpointType.BaseType;
//         var endpointBaseGenericArguments = endpointBaseType!.GetGenericArguments();
//
//         const int responseTypeIndex = 0;
//
//         var responseType = endpointBaseGenericArguments[responseTypeIndex];
//
//         var type = typeof(EndpointRouteHandlerBuilder);
//         var handleRequestMethodInfo =
//             type.GetMethod(nameof(HandleRequestForEndpointWithoutRequestAndWithResponse));
//         var handleRequestGenericMethodInfo =
//             handleRequestMethodInfo!.MakeGenericMethod(_endpointType, responseType);
//
//         var handleRequestDelegateType = Expression.GetDelegateType(
//             _endpointType,
//             typeof(HttpContext),
//             typeof(CancellationToken),
//             typeof(Task<IResult>));
//
//         var handleRequestDelegate =
//             Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);
//
//         return handleRequestDelegate;
//     }
//
//     private Delegate GetHandlerOfEndpointWithRequestAndWithoutResponse()
//     {
//         var endpointBaseType = _endpointType.BaseType;
//         var endpointBaseGenericArguments = endpointBaseType!.GetGenericArguments();
//
//         const int requestTypeIndex = 0;
//
//         var requestType = endpointBaseGenericArguments[requestTypeIndex];
//
//         var type = typeof(EndpointRouteHandlerBuilder);
//         var handleRequestMethodInfo =
//             type.GetMethod(nameof(HandleRequestForEndpointWithRequestAndWithoutResponse));
//         var handleRequestGenericMethodInfo =
//             handleRequestMethodInfo!.MakeGenericMethod(_endpointType, requestType);
//
//         var handleRequestDelegateType = Expression.GetDelegateType(
//             _endpointType,
//             requestType,
//             typeof(HttpContext),
//             typeof(CancellationToken),
//             typeof(Task<IResult>));
//
//         var handleRequestDelegate =
//             Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);
//
//         return handleRequestDelegate;
//     }
//
//     private Delegate GetHandlerOfEndpointWithRequestAndWithResponse()
//     {
//         var endpointBaseType = _endpointType.BaseType;
//         var endpointBaseGenericArguments = endpointBaseType!.GetGenericArguments();
//
//         const int requestTypeIndex = 0;
//         const int responseTypeIndex = 1;
//
//         var requestType = endpointBaseGenericArguments[requestTypeIndex];
//         var responseType = endpointBaseGenericArguments[responseTypeIndex];
//
//         var type = typeof(EndpointRouteHandlerBuilder);
//         var handleRequestMethodInfo =
//             type.GetMethod(nameof(HandleRequestForEndpointWithRequestAndWithResponse));
//         var handleRequestGenericMethodInfo =
//             handleRequestMethodInfo!.MakeGenericMethod(_endpointType, requestType, responseType);
//
//         var handleRequestDelegateType = Expression.GetDelegateType(
//             _endpointType,
//             requestType,
//             typeof(HttpContext),
//             typeof(CancellationToken),
//             typeof(Task<IResult>));
//
//         var handleRequestDelegate =
//             Delegate.CreateDelegate(handleRequestDelegateType, handleRequestGenericMethodInfo);
//
//         return handleRequestDelegate;
//     }
//
//     // public RouteHandlerBuilder MapGet(string pattern)
//     // {
//     //      var routeHandlerBuilder = _endpointRouteBuilder.MapGet(pattern, () => { return Results.Ok();});
//     //
//     //      return routeHandlerBuilder;
//     // }
//     //
//     
//     public static Task<IResult> HandleRequestForEndpointWithRequestAndWithResponse<TEndpoint, TRequest, TResponse>(
//         [NotNull, FromServices] TEndpoint endpoint,
//         [NotNull, AsParameters] TRequest request,
//         HttpContext httpContext,
//         CancellationToken cancellationToken) 
//         where TEndpoint : Endpoint.WithRequest<TRequest>.WithResponse<TResponse>
//         where TRequest : class
//         where TResponse : class
//     {
//         return endpoint.ValidateAndHandleAsync(request, httpContext, cancellationToken);
//         // return endpoint.HandleAsync(request, cancellationToken);
//     }
//     
//     public static Task<IResult> HandleRequestForEndpointWithRequestAndWithoutResponse<TEndpoint, TRequest>(
//         [NotNull, FromServices] TEndpoint endpoint,
//         [NotNull, AsParameters] TRequest request,
//         HttpContext httpContext,
//         CancellationToken cancellationToken) 
//         where TEndpoint : Endpoint.WithRequest<TRequest>.WithoutResponse
//         where TRequest : class
//     {
//         return endpoint.ValidateAndHandleAsync(request, httpContext, cancellationToken);
//         // return endpoint.HandleAsync(request, cancellationToken);
//     }
//
//     public static Task<IResult> HandleRequestForEndpointWithoutRequestAndWithResponse<TEndpoint, TResponse>(
//         [NotNull, FromServices] TEndpoint endpoint,
//         HttpContext httpContext,
//         CancellationToken cancellationToken) 
//         where TEndpoint : Endpoint.WithoutRequest.WithResponse<TResponse>
//         where TResponse : class
//     {
//         return endpoint.HandleAsync(httpContext, cancellationToken);
//     }
//
//     public static Task<IResult> HandleRequestForEndpointWithoutRequestAndWithoutResponse<TEndpoint>(
//         [NotNull, FromServices] TEndpoint endpoint,
//         HttpContext httpContext,
//         CancellationToken cancellationToken) 
//         where TEndpoint : Endpoint.WithoutRequest.WithoutResponse
//     {
//         return endpoint.HandleAsync(httpContext, cancellationToken);
//     }
// }