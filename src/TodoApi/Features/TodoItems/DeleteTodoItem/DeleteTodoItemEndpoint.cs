using TodoApi.Features.TodoItems.Models;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.DeleteTodoItem;

public class DeleteTodoItemEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public DeleteTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public async Task<IResult> HandleAsync(int id)
    {
        var todoToRemove = new Todo()
        {
            Id = id
        };

        _todoItemsDbContext.Todos.Remove(todoToRemove);
        await _todoItemsDbContext.SaveChangesAsync();

        return Results.NoContent();
    }
}