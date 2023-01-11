using Microsoft.OpenApi.Models;
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
        var apiVersionSet = builder.NewApiVersionSet().ReportApiVersions().Build();

        builder
            .MapDelete("/api/v{version:apiVersion}/todoitems/{id}", (Guid id) => HandleAsync(id))
            
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(1)
            
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            
            .WithName("DeleteTodoItem")
            .WithTags("TodoItems")
            .WithSummary("Deletes Todo item")
            .WithDescription("Deletes Todo item")
            .WithOpenApi(operation =>
            {
                return operation;
            });;
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