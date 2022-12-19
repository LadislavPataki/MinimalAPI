using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApi.Features.TodoItems.DomainModels;

namespace TodoApi.Infrastructure;

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

public class TodoItemConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(
        EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(x => x.Id);
    }
}