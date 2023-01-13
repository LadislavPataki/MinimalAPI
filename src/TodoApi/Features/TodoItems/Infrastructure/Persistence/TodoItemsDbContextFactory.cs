using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TodoApi.Features.TodoItems.Infrastructure.Persistence;

public class TodoItemsDbContextFactory : IDesignTimeDbContextFactory<TodoItemsDbContext>
{
    public TodoItemsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodoItemsDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=127.0.0.1,1433;" +
            "Database=TodoItemsDb;" +
            "User=sa;" +
            "Password=Pass@w0rd!;" +
            "MultipleActiveResultSets=true;" +
            "TrustServerCertificate=True;");
        
        return new TodoItemsDbContext(optionsBuilder.Options);
    }
}