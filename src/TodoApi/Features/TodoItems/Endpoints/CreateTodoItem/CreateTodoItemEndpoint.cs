using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using TodoApi.Features.TodoItems.DomainModels;
using TodoApi.Features.TodoItems.Infrastructure.Persistence;

namespace TodoApi.Features.TodoItems.Endpoints.CreateTodoItem;

// public class CreateTodoItemEndpoint
// : Endpoint<CreateTodoItemRequest, CreateTodoItemResponse>(POST, "/todoitems")

// public class CreateTodoItemEndpoint : Endpoint
// .WithVerb(POST)
// .WithRoute("/todoitems")
// .WithRequest<CreateTodoItemRequest>
// .WithResponse<CreateTodoItemResponse>

// public class CreateTodoItemEndpoint : Endpoint
// .WithVerb(POST)
// .WithRoute("/todoitems")
// .WithRequest<CreateTodoItemRequest>
// .WithResponse<CreateTodoItemResponse>
// .WithOpenApiOperation<OpenApiOperation>

// public class CreateTodoItemEndpoint
// : Endpoint
// .WithVersion(1)
// .WithVersion(2)
// .WithVerb(POST)
// .WithVerb(GET)
// .WithRoute("/todoitems")
// .WithRequest<CreateTodoItemRequest>
// .WithoutRequest
// .WithResponse<CreateTodoItemResponse>
// .WithoutResponse
// .WithOpenApiOperation<OpenApiOperation>
// {

// }


public class HttpPostAttribute : Attribute
{
    public HttpPostAttribute(string route)
    {
        Route = route;
    }

    public string Route { get; }
}

// public class DerivedClass : BaseClass.Something
// {
//     
// }



[HttpPost("/resources")]
public class CreateResources2Endpoint : Endpoint
    .WithRequest<CreateResourceRequest>
    .WithResponse<CreateResourceResponse>
{
    private readonly ILogger<CreateResources2Endpoint> _logger;
    public CreateResources2Endpoint(ILogger<CreateResources2Endpoint> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("/resources")]
    public override async Task<IResult> HandleAsync(
        CreateResourceRequest request, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(new CreateResourceResponse(Guid.NewGuid(), string.Empty));
    }
}

[HttpGet("/resources")]
[Authorize]
public class CreateResources3Endpoint : Endpoint
    .WithoutRequest
    .WithResponse<CreateResourceResponse>
    .WithAuthorizaion<CreateResourceAuhtorizationConfiguration>
    .WithOpenApiOperation<OpenApiOperation>
{
    private readonly ILogger<CreateResources2Endpoint> _logger;
    public CreateResources3Endpoint(ILogger<CreateResources2Endpoint> logger)
    {
        _logger = logger;
    }

    public override Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        //return new todoitem
        
        throw new NotImplementedException();
    }
}



[HttpPost("/todoitems")]
public class CreateResource1Endpoint : Endpoint
    .WithRequest<CreateResourceRequest>
    .WithResponse<CreateResourceResponse>
{
    public void Configure(IEndpointRouteBuilder builder)
    {
        builder
                
            .MapPost("/todoitems", HandleAsync)
                
                
            .MapToApiVersion(1)
                
                
            .Accepts<CreateTodoItemRequest>(MediaTypeNames.Application.Json)

            .Produces<CreateTodoItemResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            // .Produces(StatusCodes.Status400BadRequest)
            // .Produces(StatusCodes.Status409Conflict)
            // .Produces(StatusCodes.Status422UnprocessableEntity)

            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status409Conflict)

            .RequireAuthorization("some-policy");
    }

    public override async Task<IResult> HandleAsync(CreateResourceRequest request, CancellationToken cancellationToken)
    {
        return TypedResults.Conflict();
        
        var response = new CreateResourceResponse(Guid.NewGuid(), request.Prop1);
        
        return TypedResults.Ok(response);
    }
}

public abstract class EndpointBase<TRequest, TResponse>
{
    //private readonly IValidator<TRequest> _validator;
    private string _route;
    private IEnumerable<string> _verbs;


    void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        var routeHandlerBuilder = versionedApiGroup.MapMethods(_route, _verbs,
            
                HandleValidationAsync);
        
        

        // versionedApiGroup
        //     .MapPost("/todoitems",
        //         (HttpContext httpContext, TRequest request, CancellationToken cancellationToken) =>
        //             HandleValidationAsync(httpContext, request, cancellationToken));
    }
    
    public async Task<IResult> HandleValidationAsync(
        HttpContext httpContext, 
        TRequest request, 
        IValidator<TRequest> validator,
        CancellationToken cancellationToken)
    {
        // var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        
        //validationResult.Errors.Any(x => x.)
        //validationResult.Errors.Any(x => x.)
        
        if (!validationResult.IsValid) 
        {
            return Results.ValidationProblem(
                validationResult.ToDictionary(), 
                statusCode: StatusCodes.Status422UnprocessableEntity,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = Activity.Current?.Id ?? httpContext?.TraceIdentifier
                });
        }

        return await HandleAsync(request, cancellationToken);
    }

    protected abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}



//public abstract class EndpointBase{}

public static class Endpoint
{
    public static class WithRequest<TRequest>
    {
        public abstract class WithResponse<TResponse>
        {
            public abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
            //public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
        }
        
        public abstract class WithoutResponse
        {
            public abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
        }
    }
    
    public static class WithoutRequest
    {
        public abstract class WithResponse<TResponse>
        {
            public abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);
        }
        
        public abstract class WithoutResponse
        {
            public abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);
        }
    }

}




public class CreateResourceEndpoint //: IEndpoint
{
    // public void AddEndpoint(IEndpointRouteBuilder builder)
    // {
    //     throw new NotImplementedException();
    // }

    public CreateResourceEndpoint()
    {
        
    }

    public async Task<IResult> HandleAsync(CreateResourceRequest request, CancellationToken cancellationToken)
    {
        var response = new CreateResourceResponse(Guid.NewGuid(), request.Prop1);
        
        return TypedResults.Ok(response);
    }
}

public class OpenApiConfiguration
{
    
}


public class CreateResourceRequestValidator : AbstractValidator<CreateResourceRequest>
{
    public CreateResourceRequestValidator()
    {
        RuleFor(x => x.Prop1)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(15);
    }
}

public record CreateResourceResponse(Guid Id, string Prop1)
{
    
}

public record CreateResourceRequest(string Prop1)
{
}






public class CreateTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;
    private readonly IValidator<CreateTodoItemRequest> _validator;
    private readonly IOutputCacheStore _outputCacheStore;
    private int _myField;

    public CreateTodoItemEndpoint(
        TodoItemsDbContext todoItemsDbContext,
        IValidator<CreateTodoItemRequest> validator,
        IOutputCacheStore outputCacheStore)
    {
        _todoItemsDbContext = todoItemsDbContext;
        _validator = validator;
        _outputCacheStore = outputCacheStore;
        _myField = 1;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        // var routeHandlerBuilder = versionedApiGroup
        //     .MapPost("/todoitems", (CreateTodoItemRequest request, CancellationToken cancellationToken) => 
        //         HandleAsync(request, cancellationToken));
        
        var routeHandlerBuilder = versionedApiGroup
            .MapPost("/todoitems", HandleAsync);
        
        routeHandlerBuilder
            
            .MapToApiVersion(1)
            
            .WithOpenApi()
            
            

            .Accepts<CreateTodoItemRequest>(MediaTypeNames.Application.Json)

            .Produces<CreateTodoItemResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            // .Produces(StatusCodes.Status400BadRequest)
            // .Produces(StatusCodes.Status409Conflict)
            // .Produces(StatusCodes.Status422UnprocessableEntity)

            .ProducesValidationProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(statusCode: StatusCodes.Status409Conflict)

            // .WithName("CreateTodoItem")
            // .WithTags("TodoItems")
            // .WithSummary("Creates Todo item")
            // .WithDescription("Creates Todo item")
            .WithOpenApi(ConfigureOpenApiOperation);

        versionedApiGroup
            .MapPost("/todoitems",
                (HttpContext httpContext, CreateTodoItemRequest request, CancellationToken cancellationToken) => 
                    HandleValidationAsync(httpContext, request, cancellationToken))
            
            //.RequireAuthorization("todo:read-write")
            //.RequireAuthorization()
            
            .MapToApiVersion(2)
            
            .Accepts<CreateTodoItemRequest>(MediaTypeNames.Application.Json)

            .Produces<CreateTodoItemResponse>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            // .Produces(StatusCodes.Status400BadRequest)
            // .Produces(StatusCodes.Status409Conflict)
            // .Produces(StatusCodes.Status422UnprocessableEntity)

            .ProducesValidationProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(statusCode: StatusCodes.Status409Conflict)

            // .WithName("CreateTodoItem")
            // .WithTags("TodoItems")
            // .WithSummary("Creates Todo item")
            // .WithDescription("Creates Todo item")
            .WithOpenApi(ConfigureOpenApiOperation);
    }

    private static OpenApiOperation ConfigureOpenApiOperation(OpenApiOperation operation)
    {
        operation.OperationId = "CreateTodoItem";
        operation.Summary = "Creates Todo item summary";
        operation.Description = "Creates Todo item description";
        operation.Tags = new List<OpenApiTag> { new() { Name = "TodoItems" } };

        var openApiRequestBody = operation.RequestBody;
        openApiRequestBody.Description = "Request body description";
        openApiRequestBody.Content["application/json"].Example = new OpenApiString(
            JsonSerializer.Serialize(new CreateTodoItemRequest
            {
                Name = "TodoItemName",
                IsComplete = false
            }), false, true);

        var openApiCreatedResponse = operation.Responses["201"];
        openApiCreatedResponse.Description = "Success description";
        openApiCreatedResponse.Content["application/json"].Example = new OpenApiString(
            JsonSerializer.Serialize(new CreateTodoItemResponse
            {
                Id = Guid.NewGuid(),
                Name = "TodoItemName",
                IsComplete = false
            }), false, true);

        operation.Responses["400"].Description = "Client error description";
        operation.Responses["409"].Description = "Client error description";

        // operation.Security = new List<OpenApiSecurityRequirement>()
        // {
        //     new()
        //     {
        //         {
        //             new OpenApiSecurityScheme
        //             {
        //                 Reference = new OpenApiReference
        //                 {
        //                     Type = ReferenceType.SecurityScheme,
        //                     Id = "BearerAuth"
        //                 }
        //             },
        //             Array.Empty<string>()
        //         },
        //         // {
        //         //     new OpenApiSecurityScheme
        //         //     {
        //         //         Reference = new OpenApiReference
        //         //         {
        //         //             Type = ReferenceType.SecurityScheme,
        //         //             Id = "Oauth2"
        //         //         }
        //         //     },
        //         //     new List<string>() { "todo:read-write" }
        //         // }
        //     }
        // };

        return operation;
    }
    
    public async Task<IResult> HandleValidationAsync(HttpContext httpContext, CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        
        //validationResult.Errors.Any(x => x.)
        //validationResult.Errors.Any(x => x.)
        
        if (!validationResult.IsValid) 
        {
            return Results.ValidationProblem(
                validationResult.ToDictionary(), 
                statusCode: StatusCodes.Status422UnprocessableEntity,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = Activity.Current?.Id ?? httpContext?.TraceIdentifier
                });
        }

        return await HandleAsync(request, cancellationToken);
    }

    public async Task<IResult> HandleAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        _myField++;

        var local = _myField;
        
        //  //throw new NotImplementedException();
        //
        // var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        //
        // //validationResult.Errors.Any(x => x.)
        // //validationResult.Errors.Any(x => x.)
        //
        // if (!validationResult.IsValid) 
        // {
        //     return Results.ValidationProblem(
        //         validationResult.ToDictionary(), 
        //         statusCode: StatusCodes.Status422UnprocessableEntity);
        // }

        await _outputCacheStore.EvictByTagAsync("todoItems", cancellationToken);

        var todoItem = new Todo()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsComplete = request.IsComplete
        };

        _todoItemsDbContext.Todos.Add(todoItem);
        await _todoItemsDbContext.SaveChangesAsync();

        var response = new CreateTodoItemResponse()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        return TypedResults.Ok(response);
    }
}

public class CreateTodoItemRequest
{
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

public class CreateTodoItemResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

public class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(15);
    }
}