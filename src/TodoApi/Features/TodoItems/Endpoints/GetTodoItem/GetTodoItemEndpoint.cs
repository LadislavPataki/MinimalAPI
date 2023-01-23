using Microsoft.OpenApi.Models;
using TodoApi.Common.Caching;
using TodoApi.Features.TodoItems.Infrastructure.Persistence;

namespace TodoApi.Features.TodoItems.Endpoints.GetTodoItem;

public class GetTodoItemEndpoint : IEndpoint
{
    private readonly TodoItemsDbContext _todoItemsDbContext;
    private readonly IRedisCache _redisCache;

    public GetTodoItemEndpoint(
        TodoItemsDbContext todoItemsDbContext,
        IRedisCache redisCache)
    {
        _todoItemsDbContext = todoItemsDbContext;
        _redisCache = redisCache;
    }

    public void AddEndpoint(IEndpointRouteBuilder builder)
    {
        var versionedApiGroup = builder
            .MapGroup("/")
            .HasApiVersion(1)
            .HasApiVersion(2);

        versionedApiGroup
            .MapGet("/todoitems/{id}", (
                Guid id) => HandleAsync(id))

            .MapToApiVersion(1)
            .MapToApiVersion(2)

            .Produces<GetTodoItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            
            // .CacheOutput(policyBuilder => policyBuilder
            //     .SetVaryByRouteValue("id")
            //     .Expire(TimeSpan.FromSeconds(5)))

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
        // var todoItem = await _todoItemsDbContext.Todos.FindAsync(id);

        var todoItem = await _redisCache.GetOrSetCacheValueAsync(
            $"TodoItems_{id}",
            async () =>
            {
                return await _todoItemsDbContext.Todos.FindAsync(id);
            },
            TimeSpan.FromSeconds(30));

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