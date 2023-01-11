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
        var apiVersionSet = builder.NewApiVersionSet().ReportApiVersions().Build();

        builder
            .MapGet("/api/v{version:apiVersion}/todoitems", () => HandleAsync())
            
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(1)

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