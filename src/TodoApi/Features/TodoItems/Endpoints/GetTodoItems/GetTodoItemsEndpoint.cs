using Microsoft.EntityFrameworkCore;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.Endpoints.GetTodoItems;

public class GetTodoItemsEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public GetTodoItemsEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public async Task<IResult> HandleAsync()
    {
        var todoItems = await _todoItemsDbContext.Todos.ToListAsync();

        var response = todoItems.Select(x => new TodoItem()
        {
            Id = x.Id,
            Name = x.Name,
            IsComplete = x.IsComplete
        }).ToList();

        return Results.Ok(response);
    }
}

public class GetTodoItemsRequest
{
}

public class TodoItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}