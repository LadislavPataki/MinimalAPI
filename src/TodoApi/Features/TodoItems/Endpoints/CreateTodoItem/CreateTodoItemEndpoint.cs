using TodoApi.Features.TodoItems.DomainModels;
using TodoApi.Infrastructure;

namespace TodoApi.Features.TodoItems.Endpoints.CreateTodoItem;

public class CreateTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;

    public CreateTodoItemEndpoint(TodoItemsDbContext todoItemsDbContext)
    {
        _todoItemsDbContext = todoItemsDbContext;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/todoitems", (CreateTodoItemRequest request) => HandleAsync(request));
    }

    public async Task<IResult> HandleAsync(CreateTodoItemRequest request)
    {
        var todoItem = new Todo()
        {
            Id = Guid.NewGuid(),
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

        return TypedResults.Ok(response);
    }
}

public class CreateTodoItemRequest
{
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}

public class CreateTodoItemResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}