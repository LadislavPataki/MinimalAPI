using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using Asp.Versioning.Builder;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinimalApi.Features.TodoItem.CreateTodoItem;
using MinimalApi.MinimalApiEndpoints;
using MinimalApi.MinimalApiEndpoints.Extensions;
using MinimalApi.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

//ervices.AddTransient<FluentValidationSchemaFilter>();

services.AddScoped<IScopedService, ScopedService>();
// services.AddTransient<CreateTodoItemEndpoint>();
// services.AddTransient<CreateTodoItemEndpoint2>();
// services.AddTransient<PostEndpoint>();

services.AddEndpoints();

services.AddValidatorsFromAssemblyContaining(typeof(Program));
//services.AddFluentValidationAutoValidation();

// services.AddTransient<EndpointFactory>();
// services.AddTransient<EndpointFactory2>();

// builder.Services.AddControllers();
// builder.Services.AddEndpoints()

// add api versioning
services
    .AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

services.AddEndpointsApiExplorer();

// add swagger
services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());

//services.AddFluentValidationRulesToSwagger();

// add services for problem details
services.AddProblemDetails();


var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";

        if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

            if (exceptionHandlerFeature?.Error is { } exception)
            {
                var statusCode = exception switch
                {
                    BadHttpRequestException badHttpRequestException =>
                    (
                        badHttpRequestException.StatusCode
                    ),
                    _ =>
                    (
                        StatusCodes.Status500InternalServerError
                    )
                };
                
                context.Response.StatusCode = statusCode;
                
                // It calls DefaultProblemDetailsWriter.WriteAsync which applies defaults
                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context
                });
            }
        }
    });
});

app.UseStatusCodePages();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach (var description in descriptions.Reverse())
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

// app.MapControllers();


// app.MapGet("api/v1/resources", ([AsParameters] Paging paging) =>
//     {
//         var response = new CreateResourceResponse()
//         {
//             ResourceId = Guid.NewGuid(),
//             Name = request.Body.Name
//         };
//
//         return Results.Ok(response);
//     })
//     .WithSummary("Test withou [FromBody] binding");

// app.MapPost("/api/v1/todoitems/{id}", (
//         [FromServices] CreateTodoItemEndpoint createTodoItemEndpoint,
//         [AsParameters] CreateTodoItemRequest request,
//         CancellationToken cancellationToken) => createTodoItemEndpoint.HandleAsync(request, cancellationToken))
//     .Accepts<CreateTodoItemRequest>(MediaTypeNames.Application.Json)
//     .Produces<CreateTodoItemResponse>();

// app.MapPost("/api/v2/todoitems", HandleCreateTodoItemsRequest);
// app.MapPost("/api/v3/todoitems", HandleRequest<CreateTodoItemEndpoint, CreateTodoItemRequest>);
//
// var apiEndpointTypes = Assembly.GetExecutingAssembly().GetTypes()
//          .Where(type => type.IsDefined(typeof(ApiEndpointAttribute)));

//
//
// foreach (var apiEndpointType in apiEndpointTypes)
// {
//     var apiEndpointAttribute = apiEndpointType.GetCustomAttribute<ApiEndpointAttribute>();
//
//     var path = apiEndpointAttribute.Path;
//
//     var apiEndpointBaseType = apiEndpointType.BaseType;
//     var requestType = apiEndpointBaseType.GetGenericArguments().FirstOrDefault();
//
//
//     // app.MapPost(path, CreateHandlerDelegate(apiEndpoint))
//     //     .Accepts<CreateTodoItemRequest>(MediaTypeNames.Application.Json)
//     //     .Produces<CreateTodoItemResponse>();
//
// }


// app.MapEndpoints();
app.MapEndpoints();

// app.MapPost("/resources/{id}",
//     ([AsParameters] CreateTodoItemRequest request) =>
//     {
//         return Results.Ok(new CreateTodoItemResponse());
//     });


static Delegate CreateHandlerDelegate(Type type)
{
    var methods = type.GetMethods().Where(x => x.Name.Equals("Handle"));

    //var methods = type.GetMethods().Where(x => x.GetCustomAttribute<ApiEndpointAttribute>() != null);
    
    foreach (var method in methods)
    {
        var parameters = method.GetParameters();
        var delegateType = Expression.GetDelegateType(
            parameters.Select(x => x.ParameterType).Concat(new[] { method.ReturnType }).ToArray());

        
        var result = Delegate.CreateDelegate(delegateType, method);
        

        return result;
    }

    return null;
}

// app.MapPost("/api/v1/todoitems", (
//         [FromServices] CreateTodoItemEndpoint createTodoItemEndpoint) => createTodoItemEndpoint.HandleAsync)
//     .Accepts<CreateTodoItemRequest>(MediaTypeNames.Application.Json)
//     .Produces<CreateTodoItemResponse>();


// app.MapPost("api/v1/resources-with-from-body", ([AsParameters] CreateResourceRequest request) =>
//     {
//         var response = new CreateResourceResponse()
//         {
//             ResourceId = Guid.NewGuid(),
//             Name = request.Body.Name
//         };
//
//         return Results.Ok(response);
//     })
//     .WithDisplayName("Test with [FromBody] binding");
//
// app.MapPost("api/v1/resources-without-from-body", (CreateResourceBody requestBody) =>
//     {
//         var response = new CreateResourceResponse()
//         {
//             ResourceId = Guid.NewGuid(),
//             Name = requestBody.Name
//         };
//
//         return Results.Ok(response);
//     })
//     .WithDisplayName("Test without [FromBody] binding")
//     .WithName("Test without [FromBody] binding")
//     .WithDescription("Test without [FromBody] binding")
//     .WithSummary("Test without [FromBody] binding");

// IResult Handler(
//     string resourceId, 
//     [FromHeader] int? pageNumber, 
//     [FromHeader] int? pageSize,
//     [FromHeader] string sort, 
//     [FromBody] CreateResourceBody requestBody,
//     HttpContext httpContext,
//     ClaimsPrincipal user,
//     CancellationToken cancellationToken)
// {
//     
//     
//     
//     return Results.Ok("");
// }
//
// IResult Handler2(
//     string resourceId, 
//     [FromHeader] int? pageNumber, 
//     [FromHeader] int? pageSize,
//     [FromHeader] string sort,
//     CancellationToken cancellationToken)
// {
//     
//     return Results.Ok("");
// }
//
// IResult Handler3(
//     string resourceId, 
//     [AsParameters] QueryStringParameters queryStringParameters,
//     [AsParameters] HeaderParams headerParameters,
//     
//     CancellationToken cancellationToken)
// {
//     
//     return Results.Ok("");
// }

// app.MapGet("api/v1/resources/{resourceId}", Handler);




// app.MapEndpoints();
//
// app.MapEndpoints2<CreateTodoItemRequest>();
// app.MapEndpoints3<GetTodoItemRequest>();

app.Run();


// public static class WebApplicationExtensions
// {
//     // public static void MapEndpoints(this IEndpointRouteBuilder app)
//     // {
//     //     var apiEndpoints = Assembly.GetExecutingAssembly().GetTypes()
//     //         .Where(type => type.IsDefined(typeof(ApiEndpointAttribute)));
//     //
//     //     foreach (var apiEndpoint in apiEndpoints)
//     //     {
//     //         var apiEndpointAttribute = apiEndpoint.GetCustomAttribute<ApiEndpointAttribute>();
//     //
//     //         var path = apiEndpointAttribute.Path;
//     //
//     //         var apiEndpointBaseType = apiEndpoint.BaseType;
//     //         var requestType = apiEndpointBaseType.GetGenericArguments().FirstOrDefault();
//     //         
//     //         app.MapPost(path, ((apiEndpoint handler) =>
//     //         {
//     //             
//     //         }))
//     //         
//     //         // app.MapPost(path, ((
//     //         //     HttpContext context,
//     //         //     [FromServices] EndpointFactory2 endpointFactory) =>
//     //         // {
//     //         //     var concreteEndpoint = endpointFactory.Create(apiEndpoint);
//     //         //     concreteEndpoint as EndpointBase<>
//     //         //
//     //         //     return
//     //         // }))
//     //         
//     //         //var genericTypeDefinition = apiEndpointBaseType.GetGenericTypeDefinition();
//     //
//     //         // var genericArguments = apiEndpoint.GetGenericArguments();
//     //         // var genericTypeDefinition = apiEndpoint.GetGenericTypeDefinition();
//     //         
//     //         //apiEndpoint.get
//     //     }
//     //     
//     //     // app.MapPost(
//     //     //     "/api/v1/todoitems2",
//     //     //     (HttpContext httpContext, 
//     //     //         T request,
//     //     //         CancellationToken cancellationToken,
//     //     //         [FromServices] EndpointFactory2 endpointFactory) =>
//     //     //     {
//     //     //         var createTodoItemEndpoint = endpointFactory.Create();
//     //     //         var endpointBase = createTodoItemEndpoint as EndpointBase<T>;
//     //     //         endpointBase.HttpContext = httpContext;
//     //     //
//     //     //         return endpointBase.HandleAsync(request, cancellationToken);
//     //     //     });
//     //
//     //
//     //     // app.MapPost(
//     //     //     "/api/v1/todoitems",
//     //     //     (
//     //     //         HttpContext httpContext, 
//     //     //         CreateTodoItemRequest request,
//     //     //         CancellationToken cancellationToken,
//     //     //         [FromServices] EndpointFactory endpointFactory) =>
//     //     //     {
//     //     //         return endpointFactory.Create().HandleAsync(request, cancellationToken);
//     //     //     });
//     //
//     //
//     //     // app.MapMethods("/api/v1/todoitems",new[] { "POST" },
//     //     //     (HttpContext httpContext,
//     //     // [FromServices] EndpointFactory endpointFactory,
//     //     // CancellationToken cancellationToken) =>
//     //     //     {
//     //     //         return endpointFactory.Create().HandleAsync(new CreateTodoItemRequest(), cancellationToken);
//     //     //     });
//     // }
//
//     // public static void MapEndpoints2<T>(this IEndpointRouteBuilder app) where T : class
//     // {
//     //     var pluggedTypes = Assembly.GetExecutingAssembly().GetTypes()
//     //         .Where(type => !(type.IsGenericTypeDefinition || type.ContainsGenericParameters))
//     //         .ToList();
//     //
//     //     foreach (var pluggedType in pluggedTypes)
//     //     {
//     //
//     //         foreach (
//     //             var interfaceType in
//     //             pluggedType.GetInterfaces()
//     //                 .Where(type => type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(EndpointBase<>))))
//     //         {
//     //             
//     //         }
//     //         
//     //         
//     //         //pluggedType.GetGenericTypeDefinition()
//     //         
//     //         
//     //     }
//     //     
//     //     var endpoints = Assembly.GetExecutingAssembly().GetTypes()
//     //         .Where(x => typeof(EndpointBase<>).IsAssignableFrom(x) 
//     //                     && x is { IsInterface: false, IsAbstract: false })
//     //         .ToList();
//     //     
//     //     var endpoints2 = Assembly.GetExecutingAssembly().GetTypes()
//     //         .Where(x => typeof(EndpointBase<>).IsAssignableFrom(x))
//     //         .ToList();
//     //
//     //     
//     //     app.MapPost(
//     //         "/api/v1/todoitems2",
//     //         (HttpContext httpContext, 
//     //             T request,
//     //             CancellationToken cancellationToken,
//     //             [FromServices] EndpointFactory2 endpointFactory) =>
//     //         {
//     //             var createTodoItemEndpoint = endpointFactory.Create();
//     //             var endpointBase = createTodoItemEndpoint as EndpointBase<T>;
//     //             endpointBase.HttpContext = httpContext;
//     //
//     //             return endpointBase.HandleAsync(request, cancellationToken);
//     //         });
//     // }
//
//     // public static void MapEndpoints3<T>(this IEndpointRouteBuilder app) where T : class
//     // {
//     //
//     //     
//     //     app.MapGet(
//     //         "/api/v1/todoitems2",
//     //         (HttpContext httpContext, 
//     //             T request,
//     //             CancellationToken cancellationToken,
//     //             [FromServices] EndpointFactory2 endpointFactory) =>
//     //         {
//     //             var createTodoItemEndpoint = endpointFactory.Create<CreateTodoItemEndpoint>();
//     //             var endpointBase = createTodoItemEndpoint as EndpointBase<T>;
//     //             endpointBase.HttpContext = httpContext;
//     //
//     //             return endpointBase.HandleAsync(request, cancellationToken);
//     //         });
//     // }
//     
// }

public class CreateResourceRequest
{
    [FromBody()] public CreateResourceBody Body { get; set; }
    
    [FromHeader(Name = "TestHeader")] public string TestHeader { get; set; }
    [FromHeader(Name = "TestHeader2")] public string TestHeader2 { get; set; }
    
    [FromRoute] public string ResourceId { get; set; }

}

public class CreateResourceBody
{
    public string Name { get; set; }
    public NestedObject NestedObject { get; set; }
}

public class NestedObject
{
    public string NestedName { get; set; }
}

public class CreateResourceResponse
{
    public Guid ResourceId { get; set; }
    public string Name { get; set; }
}

public class Paging
{
}

public class QueryStringParameters
{
}

// public static class MyExtensions
// {
//     public static RouteHandlerBuilder MapPost(
//         this IEndpointRouteBuilder endpoints,
//         [StringSyntax("Route")] string pattern)
//     {
//         
//         
//         var routeHandlerBuilder = endpoints.MapPost("", () => { return Results.Ok();});
//         
//         
//
//         return routeHandlerBuilder;
//     }
//
//
//     // public static RouteHandlerBuilder MapPost(
//     //     this IMyRouteBuilder builder,
//     //     [StringSyntax("Route")] string pattern)
//     // {
//     //     var endpoints = builder as IEndpointRouteBuilder;
//     //
//     //     var routeHandlerBuilder = endpoints.MapPost("", () => { return Results.Ok();});
//     //
//     //     return routeHandlerBuilder;
//     // }
//     
//     
//     
//     
//     
// }


// public class MyRouteBuilder //: IEndpointRouteBuilder
// {
//     public IApplicationBuilder CreateApplicationBuilder()
//     {
//         throw new NotImplementedException();
//     }
//
//     public IServiceProvider ServiceProvider { get; }
//     public ICollection<EndpointDataSource> DataSources { get; }
// }
//
//
//
// public static class MyRoutBuilderExtensions
// {
//     public static RouteHandlerBuilder MapPost(
//         this MyRouteBuilder endpoints,
//         [StringSyntax("Route")] string pattern)
//     {
//         var routeHandlerBuilder = endpoints.MapPost(pattern, () => { return Results.Ok();});
//
//         return routeHandlerBuilder;
//     }
// }

// public abstract class MyListBase
// {
//     
// }
//
// public abstract class MyList<> where T : class
// {
//     
// }


// public interface IMyRouteBuilder
// {
//     public RouteHandlerBuilder MapPost([StringSyntax("Route")] string pattern);
// }
