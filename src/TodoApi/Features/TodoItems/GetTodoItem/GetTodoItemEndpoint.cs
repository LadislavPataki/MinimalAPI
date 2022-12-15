using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.GetTodoItem;

public class GetTodoItemEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public GetTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public async Task<IResult> HandleAsync(int id)
    {
        var todoItem = await _todoItemsDbContext.Todos.FindAsync(id);

        if (todoItem is null)
            return Results.NotFound();

        var response = new TodoItem()
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };

        return Results.Ok(response);
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}