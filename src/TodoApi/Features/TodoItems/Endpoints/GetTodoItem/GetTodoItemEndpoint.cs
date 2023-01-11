using Microsoft.OpenApi.Models;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.Endpoints.GetTodoItem;

public class GetTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public GetTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var apiVersionSet = builder.NewApiVersionSet().ReportApiVersions().Build();

        builder
            .MapGet("/api/v{version:apiVersion}/todoitems/{id}", (Guid id) => HandleAsync(id))
            
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(1)

            .Produces<GetTodoItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            
            .WithName("GetTodoItem")
            .WithTags("TodoItems")
            .WithSummary("Gets Todo item")
            .WithDescription("Gets Todo item")
            .WithOpenApi(operation =>
            {
                var parameter = operation.Parameters[0];
                parameter.Description = "The ID associated with the created Todo";


                return operation;
            });
    }

    public async Task<IResult> HandleAsync(Guid id)
    {
        var todoItem = await _todoItemsDbContext.Todos.FindAsync(id);

        if (todoItem is null)
            return TypedResults.NotFound();

        var response = new GetTodoItemResponse()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        return TypedResults.Ok(response);
    }
}

public class GetTodoItemResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}