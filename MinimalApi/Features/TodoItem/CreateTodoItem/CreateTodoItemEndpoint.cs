using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MinimalApi.MinimalApiEndpoints;
using Endpoint = MinimalApi.MinimalApiEndpoints.Endpoint;

namespace MinimalApi.Features.TodoItem.CreateTodoItem;

// public abstract class EndpointWithRequestAndWithResponse<TRequest, TResponse> : EndpointBase
//     where TRequest : class
//     where TResponse : class
// {
// }
//
// public abstract class EndpointWithoutRequestAndWithResponse<TResponse> : EndpointBase
//     where TResponse : class
//
// {
// }
//
// public abstract class EndpointWithRequestAndWithoutResponse<TRequest> : EndpointBase
//     where TRequest : class
// {
// }
//
// public abstract class EndpointWithoutRequestAndWithoutResponse : EndpointBase
// {
// }

public class GetTodoItemEndpoint : EndpointBaseGeneric<GetTodoItemRequest>
{
    public override Task<IResult> HandleAsync(GetTodoItemRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public override void ConfigureEndpoint(RouteHandlerBuilder builder)
    {
        throw new NotImplementedException();
    }
}

public class GetTodoItemRequest
{
    [FromRoute] public int Id { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ApiEndpointAttribute : Attribute
{
    public string Pattern { get; }
    public string HttpMethod { get; }

    public ApiEndpointAttribute(
        [StringSyntax("Route")] string pattern, string httpMethod)
    {
        Pattern = pattern;
        HttpMethod = httpMethod;
    }
}

public class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.CreateTodoItemsRequestBody.Name)
            .NotEmpty();
    }
}

public class GetEndpoint : Endpoint
    .WithRequest<GetTodoItemRequest>
    .WithoutResponse
{
    public GetEndpoint()
    {
        
    }
    
    public override void ConfigureEndpoint(EndpointRouteHandlerBuilder builder)
    {
        builder
            .MapGet("/resources/{id}")
            .MapToApiVersion(1)
            .AllowAnonymous();
    }

    protected override async Task<IResult> HandleAsync(GetTodoItemRequest request, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }
}

public class PostEndpoint : Endpoint
    .WithRequest<CreateTodoItemRequest>
    .WithResponse<CreateTodoItemResponse>
{
    private readonly IScopedService _scopedService;
    private int _myField;
    
    public PostEndpoint(IScopedService scopedService)
    {
        _scopedService = scopedService;
    }
    
    public override void ConfigureEndpoint(EndpointRouteHandlerBuilder builder)
    {
        builder
            .MapPost("/resources/{id}")
            .MapToApiVersion(1)
            .Accepts<CreateTodoItemsRequestBody>(MediaTypeNames.Application.Json)

            .Produces<CreateTodoItemResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)

            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status502BadGateway)

            // .RequireAuthorization()
            .AllowAnonymous();

            // .WithOpenApi(operation =>
            //     {
            //         operation.OperationId = "CreateTodoItem";
            //         operation.Summary = "Creates Todo item summary";
            //         operation.Description = "Creates Todo item description";
            //         operation.Tags = new List<OpenApiTag> { new() { Name = "TodoItems" } };
            //
            //         return operation;
            //     }
            //
            // );
    }

    protected override async Task<IResult> HandleAsync(
        CreateTodoItemRequest request, 
        CancellationToken cancellationToken)
    {
        _myField++;

        var local = _myField;

        var list = _scopedService.GetList();
        
        _scopedService.Add("1");


        return Results.Ok();
    }
}

[ApiEndpoint("/v27/todoitems/{id}", "POST")]
public class CreateTodoItemEndpoint 
    : EndpointBaseGeneric<CreateTodoItemRequest>
{
    private readonly IScopedService _scopedService;
    private int _myField;

    public CreateTodoItemEndpoint(IScopedService scopedService)
    {
        _scopedService = scopedService;
        _myField = 1;
    }

    public static void ConfigureEndpointTest(EndpointRouteHandlerBuilder handlerBuilder)
    {
        handlerBuilder
            .MapPost("")
            .MapToApiVersion(1)
            .AllowAnonymous();
    }
    
    
    public override void ConfigureEndpoint(RouteHandlerBuilder builder)
    {
        builder
            .MapToApiVersion(1)
            .Accepts<CreateTodoItemsRequestBody>(MediaTypeNames.Application.Json)

            .Produces<CreateTodoItemResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)

            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status502BadGateway)

            // .RequireAuthorization()
            .AllowAnonymous()

            .WithOpenApi(operation =>
                {
                    operation.OperationId = "CreateTodoItem";
                    operation.Summary = "Creates Todo item summary";
                    operation.Description = "Creates Todo item description";
                    operation.Tags = new List<OpenApiTag> { new() { Name = "TodoItems" } };

                    return operation;
                }

            );
    }

    public IResult Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var todoItem = new Todo()
        {
            Id = Guid.NewGuid(),
            Name = request.CreateTodoItemsRequestBody.Name,
            IsComplete = request.CreateTodoItemsRequestBody.IsComplete
        };
        
        var response = new CreateTodoItemResponse()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        return TypedResults.Ok(response);
    }
    
    public override async Task<IResult> HandleAsync(
        CreateTodoItemRequest request, 
        CancellationToken cancellationToken)
    {
        _myField++;

        var local = _myField;


        var todoItem = new Todo()
        {
            Id = Guid.NewGuid(),
            Name = request.CreateTodoItemsRequestBody.Name,
            IsComplete = request.CreateTodoItemsRequestBody.IsComplete
        };

        var list = _scopedService.GetList();

        _scopedService.Add(todoItem.Name);

        var response = new CreateTodoItemResponse()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        return TypedResults.Ok(response);
    }
}


[ApiEndpoint("/api/v37/todoitems/{id}", "POST")]
public class CreateTodoItemEndpoint2 
    : EndpointBaseGeneric<CreateTodoItemRequest2>
{
    private readonly IScopedService _scopedService;
    private int _myField;

    public CreateTodoItemEndpoint2(IScopedService scopedService)
    {
        _scopedService = scopedService;
        _myField = 1;
    }
    
    public override async Task<IResult> HandleAsync(
        CreateTodoItemRequest2 request, 
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(new CreateTodoItemResponse());
    }

    public override void ConfigureEndpoint(RouteHandlerBuilder builder)
    {
        
    }
}

public interface IConfigureEndpoint
{
    static abstract void ConfigureEndpoint(RouteHandlerBuilder builder);
}

class ConfigureEndpointImplementation : IConfigureEndpoint
{
    public static void ConfigureEndpoint(RouteHandlerBuilder builder)
    {
        throw new NotImplementedException();
    }
}

public abstract class EndpointBaseGeneric<TRequest> 
{
    public HttpContext HttpContext { get; set; }
    
    
    
    public abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);

    public abstract void ConfigureEndpoint(RouteHandlerBuilder builder);


}

public class CreateTodoItemsRequestBody
{
    public string Name { get; set; }
    public string IsComplete { get; set; }
}

public class CreateTodoItemRequest
{
    // [FromRoute(Name = "id")] public string Id { get; set; }
    [FromRoute] public string Id { get; set; }
    [FromBody] public CreateTodoItemsRequestBody CreateTodoItemsRequestBody { get; set; }
}

public class CreateTodoItemRequest2
{
    //public string RandomProperty { get; set; }
    
    [FromRoute(Name = "id")] public string Id { get; set; }
    // [FromBody] public CreateTodoItemsRequestBody CreateTodoItemsRequestBody { get; set; }
}

public record CreateTodoItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string IsComplete { get; set; }
}

public record Todo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string IsComplete { get; set; }
}

public interface IScopedService
{
    void Add(string item);
    List<string> GetList();
    
}

public class ScopedService : IScopedService
{
    private readonly List<string> _list;

    public ScopedService()
    {
        _list = new List<string>();
    }
    
    public void Add(string item)
    {
        _list.Add(item);
    }

    public List<string> GetList()
    {
        return _list;
    }
}



// public class EndpointFactory
// {
//     private readonly IServiceProvider _serviceProvider;
//
//     public EndpointFactory(IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//     
//     public CreateTodoItemEndpoint Create()
//     {
//         return _serviceProvider.GetService<CreateTodoItemEndpoint>();
//     }
// }

// public class EndpointFactory2
// {
//     private readonly IServiceProvider _serviceProvider;
//
//     public EndpointFactory2(
//         IServiceProvider serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//     }
//     
//     // public CreateTodoItemEndpoint Create()
//     // {
//     //     return _serviceProvider.GetService<CreateTodoItemEndpoint>();
//     // }
//
//
//     public object Create(Type endpointType)
//     {
//         return _serviceProvider.GetService(endpointType);
//     }
    
    
