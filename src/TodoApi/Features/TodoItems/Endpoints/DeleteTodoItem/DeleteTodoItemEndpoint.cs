using TodoApi.Features.TodoItems.DomainModels;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.Endpoints.DeleteTodoItem;

public class DeleteTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public DeleteTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/todoitems/{id}", (Guid id) => HandleAsync(id));
    }

    public async Task<IResult> HandleAsync(Guid id)
    {
        var todoToRemove = new Todo()
        {
            Id = id
        };

        _todoItemsDbContext.Todos.Remove(todoToRemove);
        await _todoItemsDbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}