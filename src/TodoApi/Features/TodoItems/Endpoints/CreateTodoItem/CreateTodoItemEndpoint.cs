using System.Text.Json;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using TodoApi.Features.TodoItems.DomainModels;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.Endpoints.CreateTodoItem;

public class CreateTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public CreateTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        versionedApiGroup
            .MapPost("/todoitems", (CreateTodoItemRequest request) => HandleAsync(request))
            
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
                (CreateTodoItemRequest request) => HandleAsync(request))
            
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

        return operation;
    }

    public async Task<IResult> HandleAsync(CreateTodoItemRequest request)
    {
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