using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiEndpoints.Extensions;

public static class WebApplicationExtensions
{
    public static void MapEndpoints(this WebApplication app, Action<MapEndpointsOptions>? options = null)
    {
        var mapEndpointsOptions = new MapEndpointsOptions();
        options?.Invoke(mapEndpointsOptions);
        
        var versionedEndpointRouteBuilder = app.NewVersionedApi();
        var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup(mapEndpointsOptions.Prefix);
        
        // var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
        var endpointTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(EndpointBase).IsAssignableFrom(x)
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var apiEndpointType in endpointTypes)
        {
            var serviceScope = app.Services.CreateScope();
            var apiEndpointInstance = serviceScope.ServiceProvider.GetRequiredService(apiEndpointType);

            var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBase.ConfigureEndpoint));
            configureEndpointMethodInfo!.Invoke(
                apiEndpointInstance,
                new object?[]
                {
                    new EndpointRouteHandlerBuilder(
                        versionedApiRouteGroupBuilder,
                        apiEndpointType)
                });
        }
    }
    
    public static void MapEndpointsWithoutVersioning(this WebApplication app, Action<MapEndpointsOptions>? options = null)
    {
        var mapEndpointsOptions = new MapEndpointsOptions();
        options?.Invoke(mapEndpointsOptions);

        var versionedApiRouteGroupBuilder = app;
        // var versionedEndpointRouteBuilder = app.NewVersionedApi();
        // var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup(mapEndpointsOptions.Prefix);
        
        // var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
        var endpointTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(EndpointBase).IsAssignableFrom(x)
                        && x is { IsInterface: false, IsAbstract: false });

        foreach (var apiEndpointType in endpointTypes)
        {
            var serviceScope = app.Services.CreateScope();
            var apiEndpointInstance = serviceScope.ServiceProvider.GetRequiredService(apiEndpointType);

            var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBase.ConfigureEndpoint));
            configureEndpointMethodInfo!.Invoke(
                apiEndpointInstance,
                new object?[]
                {
                    new EndpointRouteHandlerBuilder(
                        versionedApiRouteGroupBuilder,
                        apiEndpointType)
                });
        }
    }
}

//
// public static class WebApplicationExtensions
// {
//     public static void MapEndpoints(this WebApplication app)
//     {
//         
//         var versionedEndpointRouteBuilder = app.NewVersionedApi();
//         var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup("/api/v{version:apiVersion}");
//         
//         
//         versionedApiRouteGroupBuilder
//             .MapPost("/todoitems/{id}", ([AsParameters] CreateTodoItemRequest request) =>
//             {
//                 return Results.Ok(new CreateTodoItemResponse());
//             })
//             .MapToApiVersion(1);
//         
//         
//         // app.MapPost("/api/v1/todoitems/{id}", ([AsParameters] CreateTodoItemRequest request) =>
//         // {
//         //     return Results.Ok(new CreateTodoItemResponse());
//         // });
//         //
//         //
//         // app.MapPost("/api/v2/todoitems/{id}", HandleCreateTodoItemsRequest);
//         //
//         // app.MapPost("/api/v4/todoitems/{id}", HandleCreateTodoItemsRequest2);
//         //
//         //
//         // app.MapPost("/api/v13/todoitems/{id}", HandleRequest<CreateTodoItemEndpoint, CreateTodoItemRequest>);
//         // app.MapPost("/api/v3/todoitems/{id}", HandleRequest2<CreateTodoItemEndpoint2, CreateTodoItemRequest2>);
//         //
//         // app.MapGet("/api/v1/todoitems/{id}", ([AsParameters] GetTodoItemRequest request) =>
//         // {
//         //     return Results.Ok(request.Id);
//         // });
//
//
//         //app.MapPost("", HandleCreateTodoItemsRequest);
//         //app.MapPost("/api/v3/todoitems/{id}", HandleRequest<CreateTodoItemEndpoint, CreateTodoItemRequest>);
//
//
//         var type = typeof(WebApplicationExtensions);
//         var methodInfo = type.GetMethod(nameof(HandleRequest));
//         //  
//         //  var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(CreateTodoItemEndpoint), typeof(CreateTodoItemRequest));
//         //
//         //
//         //  var delegateType = Expression.GetDelegateType(typeof(CreateTodoItemEndpoint), typeof(CreateTodoItemRequest),
//         //      typeof(CancellationToken), typeof(Task<IResult>));
//         //
//         //  var @delegate = Delegate.CreateDelegate(delegateType,genericMethodInfo);
//         //
//         //  Func<CreateTodoItemEndpoint, CreateTodoItemRequest, CancellationToken, Task<IResult>> func =
//         //      (endpoint, request, arg3) => { return null;};
//         //  
//         //  
//         //
//         //  app.MapPost("/api/v4/todoitems/{id}", func);
//         //  app.MapPost("/api/v5/todoitems/{id}", @delegate);
//         //  
//         //  
//         //  
//         var apiEndpoints = Assembly.GetExecutingAssembly().GetTypes()
//             .Where(type => type.IsDefined(typeof(ApiEndpointAttribute)));
//         
//         foreach (var apiEndpointType in apiEndpoints)
//         {
//             var apiEndpointAttribute = apiEndpointType.GetCustomAttribute<ApiEndpointAttribute>();
//         
//             var path = apiEndpointAttribute.Pattern;
//             var method = apiEndpointAttribute.HttpMethod;
//             var apiEndpointBaseType = apiEndpointType.BaseType;
//             var requestType = apiEndpointBaseType.GetGenericArguments().FirstOrDefault();
//             
//             
//             var genericMethodInfoForApiEndpoint = methodInfo.MakeGenericMethod(apiEndpointType, requestType);
//         
//         
//             var delegateTypeForApiEndpointHandler = Expression.GetDelegateType(apiEndpointType, requestType,
//                 typeof(CancellationToken), typeof(Task<IResult>));
//         
//             var apiEndpointHandlerDelegate = Delegate.CreateDelegate(delegateTypeForApiEndpointHandler, genericMethodInfoForApiEndpoint);
//
//             var routeHandlerBuilder = versionedApiRouteGroupBuilder
//                 .MapMethods(path, new[] { method }, apiEndpointHandlerDelegate);
//
//             var serviceScope = app.Services.CreateScope();
//             var apiEndpointInstance = serviceScope.ServiceProvider.GetRequiredService(apiEndpointType);
//
//             // var apiEndpointInstance = app.Services.GetRequiredService(apiEndpointType);
//             
//             
//             
//             
//             // var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBaseGeneric<object>.ConfigureEndpoint));
//             // configureEndpointMethodInfo.Invoke(apiEndpointInstance, new object?[] { routeHandlerBuilder });
//             //
//
//             
//             // var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBaseGeneric<object>.ConfigureEndpoint));
//             // configureEndpointMethodInfo.Invoke(apiEndpointInstance, new object?[] { new MyRouteHandlerBuilder(versionedEndpointRouteBuilder) });
//
//
//             //CreateTodoItemEndpoint.ConfigureEndpoint(routeHandlerBuilder);
//
//
//             //CreateTodoItemEndpoint.ConfigureEndpointTest(new MyRouteHandlerBuilder(versionedEndpointRouteBuilder));
//         }
//
//     }
//     
//     public static void MapEndpoints2(this WebApplication app)
//     {
//         var versionedEndpointRouteBuilder = app.NewVersionedApi();
//         var versionedApiRouteGroupBuilder = versionedEndpointRouteBuilder.MapGroup("/api/v{version:apiVersion}");
//         
//         // versionedApiRouteGroupBuilder
//         //     .MapPost("/todoitems/{id}", ([AsParameters] CreateTodoItemRequest request) =>
//         // {
//         //     return Results.Ok(new CreateTodoItemResponse());
//         // })
//         //     .MapToApiVersion(1);
//         //
//         
//         // app.MapPost("/api/v1/todoitems/{id}", ([AsParameters] CreateTodoItemRequest request) =>
//         // {
//         //     return Results.Ok(new CreateTodoItemResponse());
//         // });
//         //
//         //
//         // app.MapPost("/api/v2/todoitems/{id}", HandleCreateTodoItemsRequest);
//         //
//         // app.MapPost("/api/v4/todoitems/{id}", HandleCreateTodoItemsRequest2);
//         //
//         //
//         // app.MapPost("/api/v13/todoitems/{id}", HandleRequest<CreateTodoItemEndpoint, CreateTodoItemRequest>);
//         // app.MapPost("/api/v3/todoitems/{id}", HandleRequest2<CreateTodoItemEndpoint2, CreateTodoItemRequest2>);
//         //
//         // app.MapGet("/api/v1/todoitems/{id}", ([AsParameters] GetTodoItemRequest request) =>
//         // {
//         //     return Results.Ok(request.Id);
//         // });
//
//
//         //app.MapPost("", HandleCreateTodoItemsRequest);
//         //app.MapPost("/api/v3/todoitems/{id}", HandleRequest<CreateTodoItemEndpoint, CreateTodoItemRequest>);
//
//
//         // var type = typeof(WebApplicationExtensions);
//         // var methodInfo = type.GetMethod(nameof(HandleRequest));
//         // //  
//         //  var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(CreateTodoItemEndpoint), typeof(CreateTodoItemRequest));
//         //
//         //
//         //  var delegateType = Expression.GetDelegateType(typeof(CreateTodoItemEndpoint), typeof(CreateTodoItemRequest),
//         //      typeof(CancellationToken), typeof(Task<IResult>));
//         //
//         //  var @delegate = Delegate.CreateDelegate(delegateType,genericMethodInfo);
//         //
//         //  Func<CreateTodoItemEndpoint, CreateTodoItemRequest, CancellationToken, Task<IResult>> func =
//         //      (endpoint, request, arg3) => { return null;};
//         //  
//         //  
//         //
//         //  app.MapPost("/api/v4/todoitems/{id}", func);
//         //  app.MapPost("/api/v5/todoitems/{id}", @delegate);
//         //  
//         //  
//         //  
//         // var apiEndpoints = Assembly.GetExecutingAssembly().GetTypes()
//         //   .Where(type => type.IsDefined(typeof(ApiEndpointAttribute)));
//         //
//         var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
//             .Where(x => typeof(EndpointBase).IsAssignableFrom(x) 
//                         && x is { IsInterface: false, IsAbstract: false });
//         
//         foreach (var apiEndpointType in endpointTypes)
//         {
//             var serviceScope = app.Services.CreateScope();
//             var apiEndpointInstance = serviceScope.ServiceProvider.GetRequiredService(apiEndpointType);
//
//             var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBaseGeneric<object>.ConfigureEndpoint));
//             configureEndpointMethodInfo.Invoke(
//                 apiEndpointInstance,
//                 new object?[]
//                 {
//                     new EndpointRouteHandlerBuilder(
//                         versionedApiRouteGroupBuilder,
//                         apiEndpointType)
//                 });
//
//
//             
//             
//             
//             
//             
//             // var apiEndpointAttribute = apiEndpointType.GetCustomAttribute<ApiEndpointAttribute>();
//             //
//             // var path = apiEndpointAttribute.Pattern;
//             // var method = apiEndpointAttribute.HttpMethod;
//             // var apiEndpointBaseType = apiEndpointType.BaseType;
//             // var requestType = apiEndpointBaseType.GetGenericArguments().FirstOrDefault();
//             //
//             //
//             // var genericMethodInfoForApiEndpoint = methodInfo.MakeGenericMethod(apiEndpointType, requestType);
//             //
//             //
//             // var delegateTypeForApiEndpointHandler = Expression.GetDelegateType(apiEndpointType, requestType,
//             //     typeof(CancellationToken), typeof(Task<IResult>));
//             //
//             // var apiEndpointHandlerDelegate = Delegate.CreateDelegate(delegateTypeForApiEndpointHandler, genericMethodInfoForApiEndpoint);
//             //
//             // var routeHandlerBuilder = versionedApiRouteGroupBuilder
//             //     .MapMethods(path, new[] { method }, apiEndpointHandlerDelegate);
//             //
//             // var serviceScope = app.Services.CreateScope();
//             // var apiEndpointInstance = serviceScope.ServiceProvider.GetRequiredService(apiEndpointType);
//             //
//             // // var apiEndpointInstance = app.Services.GetRequiredService(apiEndpointType);
//             //
//             //
//             //
//             //
//             // // var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBaseGeneric<object>.ConfigureEndpoint));
//             // // configureEndpointMethodInfo.Invoke(apiEndpointInstance, new object?[] { routeHandlerBuilder });
//             // //
//             //
//             //
//             // var configureEndpointMethodInfo = apiEndpointType.GetMethod(nameof(EndpointBaseGeneric<object>.ConfigureEndpoint));
//             // configureEndpointMethodInfo.Invoke(apiEndpointInstance, new object?[] { new MyRouteHandlerBuilder(versionedEndpointRouteBuilder) });
//             //
//             //
//             // //CreateTodoItemEndpoint.ConfigureEndpoint(routeHandlerBuilder);
//             //
//             //
//             // //CreateTodoItemEndpoint.ConfigureEndpointTest(new MyRouteHandlerBuilder(versionedEndpointRouteBuilder));
//         }
//
//     }
//
//     public static Task<IResult> HandleCreateTodoItemsRequest(
//         [FromServices] CreateTodoItemEndpoint endpoint,
//         [AsParameters] CreateTodoItemRequest request,
//         CancellationToken cancellationToken)
//     {
//         return endpoint.HandleAsync(request, cancellationToken);
//     }
//
//     public static Task<IResult> HandleCreateTodoItemsRequest2(
//         [FromServices] CreateTodoItemEndpoint2 endpoint,
//         [AsParameters] CreateTodoItemRequest2 request,
//         CancellationToken cancellationToken)
//     {
//         return endpoint.HandleAsync(request, cancellationToken);
//     }
//
//     public static Task<IResult> HandleRequest<TEndpoint, TRequest>(
//         [NotNull, FromServices] TEndpoint endpoint,
//         [NotNull, AsParameters] TRequest request,
//         CancellationToken cancellationToken) 
//         where TEndpoint : EndpointBaseGeneric<TRequest>
//         where TRequest : class
//     {
//         ArgumentNullException.ThrowIfNull(endpoint);  
//         ArgumentNullException.ThrowIfNull(request);   
//         
//         return endpoint.HandleAsync(request, cancellationToken);
//     }
//
//     public static Task<IResult> HandleRequest2<TEndpoint, TRequest>(
//         [FromServices] TEndpoint endpoint,
//         TRequest request,
//         CancellationToken cancellationToken) where TEndpoint : EndpointBaseGeneric<TRequest>
//     {
//         return endpoint.HandleAsync(request, cancellationToken);
//     }
// }