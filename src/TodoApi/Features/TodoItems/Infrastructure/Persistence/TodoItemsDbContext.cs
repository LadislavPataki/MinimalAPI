using Microsoft.EntityFrameworkCore;
using TodoApi.Features.TodoItems.DomainModels;

namespace TodoApi.Features.TodoItems.Infrastructure.Persistence;

public class TodoItemsDbContext : DbContext
{
    public TodoItemsDbContext(DbContextOptions<TodoItemsDbContext> options) 
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoItemsDbContext).Assembly);
    }
}