using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.Endpoints.GetTodoItems;

public class GetTodoItemsEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public GetTodoItemsEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        var apiVersion1 = versionedApiGroup
            .MapGroup("/")
            .HasApiVersion(1);

        apiVersion1
            .MapGet("/todoitems", () => HandleAsync())

            .MapToApiVersion(1)
            .MapToApiVersion(2)

            .Produces<List<GetTodoItemsResponseItem>>(StatusCodes.Status200OK)

            .WithName("GetTodoItems")
            .WithTags("TodoItems")
            .WithSummary("Gets Todo items")
            .WithDescription("Gets Todo items")
            .WithOpenApi(operation =>
            {
                return operation;
            });
    }

    public async Task<IResult> HandleAsync()
    {
        var todoItems = await _todoItemsDbContext.Todos.ToListAsync();

        var response = todoItems.Select(x => new GetTodoItemsResponseItem()
        {
            Id = x.Id,
            Name = x.Name,
            IsComplete = x.IsComplete
        }).ToList();

        return TypedResults.Ok(response);
    }
}

public class GetTodoItemsRequest
{
}

public class GetTodoItemsResponseItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}