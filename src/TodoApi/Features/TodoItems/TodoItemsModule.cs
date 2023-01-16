using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TodoApi.Common.Caching;
using TodoApi.Features.TodoItems.Infrastructure.Persistence;

namespace TodoApi.Features.TodoItems;

public class TodoItemsModule : IModule
{
    public IServiceCollection AddModule(IServiceCollection services, IConfiguration configuration)
    {
        // add db context
        services.AddDbContext<TodoItemsDbContext>(options =>
        {
            var dbConnectionString = configuration.GetConnectionString("TodoItemsDbConnection");
            options.UseSqlServer(
                dbConnectionString ?? throw new InvalidOperationException("Missing TodoItemsDbConnection"),
                sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(typeof(TodoItemsDbContext).Assembly.FullName);
                    // sqlServerOptions.MigrationsHistoryTable(
                    //     TelematicsServiceDbContext.MigrationsHistoryTable,
                    //     TelematicsServiceDbContext.DbSchema);
                });
        });

        // using IDistributedCache interface
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     var connectionString = configuration.GetConnectionString("TodoItemsRedisConnection");
        //     options.Configuration = connectionString;
        //     options.InstanceName = "TodoItemsCacheInstance";
        // });

        // add redis cache
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConnectionString = configuration.GetConnectionString("TodoItemsRedisConnection");
            return ConnectionMultiplexer.Connect(
                redisConnectionString ?? throw new InvalidOperationException("Missing TodoItemsRedisConnection"));
        });
        services.AddSingleton<IRedisCache, RedisCache>();

        // maybe add module endpoints here instead of Program.cs
        // create ITodoItemsModule : IModule marker interface
        //services.AddEndpoints();
        
        return services;
    }
}