using TodoApi.Features.TodoItems.Models;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.CreateTodoItem;

public class CreateTodoItemEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public CreateTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public async Task<IResult> HandleAsync(CreateTodoItemRequest request)
    {
        var todoItem = new Todo()
        {
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

        return Results.Ok(response);
    }
}

public class CreateTodoItemRequest
{
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

public class CreateTodoItemResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}