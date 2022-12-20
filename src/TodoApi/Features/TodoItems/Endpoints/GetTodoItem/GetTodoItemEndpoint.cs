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
        builder.MapGet("/todoitems/{id}", (Guid id) => HandleAsync(id));
    }

    public async Task<IResult> HandleAsync(Guid id)
    {
        var todoItem = await _todoItemsDbContext.Todos.FindAsync(id);

        if (todoItem is null)
            return TypedResults.NotFound();

        var response = new TodoItem()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        return TypedResults.Ok(response);
    }
}

public class TodoItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}