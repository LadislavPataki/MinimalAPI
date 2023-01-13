using Microsoft.OpenApi.Models;
using TodoApi.Features.TodoItems.DomainModels;
using TodoApi.Features.TodoItems.Infrastructure.Persistence;

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
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        versionedApiGroup
            .MapDelete("/todoitems/{id}", (Guid id) => HandleAsync(id))

            .MapToApiVersion(1)
            .MapToApiVersion(2)

            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)

            .WithName("DeleteTodoItem")
            .WithTags("TodoItems")
            .WithSummary("Deletes Todo item")
            .WithDescription("Deletes Todo item")
            .WithOpenApi(operation =>
            {
                return operation;
            });
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