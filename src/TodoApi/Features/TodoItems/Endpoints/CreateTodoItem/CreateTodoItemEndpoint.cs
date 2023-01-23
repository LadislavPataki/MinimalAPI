using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using TodoApi.Features.TodoItems.DomainModels;
using TodoApi.Features.TodoItems.Infrastructure.Persistence;

namespace TodoApi.Features.TodoItems.Endpoints.CreateTodoItem;

public class CreateTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;
    private readonly IValidator<CreateTodoItemRequest> _validator;
    private readonly IOutputCacheStore _outputCacheStore;

    public CreateTodoItemEndpoint(
        TodoItemsDbContext todoItemsDbContext,
        IValidator<CreateTodoItemRequest> validator,
        IOutputCacheStore outputCacheStore)
    {
        _todoItemsDbContext = todoItemsDbContext;
        _validator = validator;
        _outputCacheStore = outputCacheStore;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        versionedApiGroup
            .MapPost("/todoitems", (CreateTodoItemRequest request, CancellationToken cancellationToken) => 
                HandleAsync(request, cancellationToken))
            
            .MapToApiVersion(1)

            .Accepts<CreateTodoItemRequest>("application/json")

            .Produces<CreateTodoItemResponse>(StatusCodes.Status201Created, "application/json")
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
                (CreateTodoItemRequest request, CancellationToken cancellationToken) => 
                    HandleAsync(request, cancellationToken))
            
            .RequireAuthorization("todo:read-write")
            //.RequireAuthorization()
            
            .MapToApiVersion(2)
            
            .Accepts<CreateTodoItemRequest>("application/json")

            .Produces<CreateTodoItemResponse>(StatusCodes.Status201Created, "application/json")
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

    public async Task<IResult> HandleAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid) 
        {
            return Results.ValidationProblem(
                validationResult.ToDictionary(), 
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

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