using Microsoft.EntityFrameworkCore;
using TodoApi.Features.TodoItems.Infrastructure.Persistence;

namespace TodoApi.Features.TodoItems;

public class TodoItemsModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        // add db context
        services.AddDbContext<TodoItemsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("TodoItemsDbConnection");
            options.UseSqlServer(connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(typeof(TodoItemsDbContext).Assembly.FullName);
                    // sqlServerOptions.MigrationsHistoryTable(
                    //     TelematicsServiceDbContext.MigrationsHistoryTable,
                    //     TelematicsServiceDbContext.DbSchema);
                });
        });

        // maybe add module endpoints here instead of Program.cs
        // create ITodoItemsModule : IModule marker interface
        //services.AddEndpoints();
        
        return services;
    }
}