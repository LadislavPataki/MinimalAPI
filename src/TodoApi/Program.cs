global using TodoApi.Common;

using Microsoft.EntityFrameworkCore;
using TodoApi.Features.TodoItems.Endpoints.CreateTodoItem;
using TodoApi.Features.TodoItems.Endpoints.DeleteTodoItem;
using TodoApi.Features.TodoItems.Endpoints.GetTodoItem;
using TodoApi.Features.TodoItems.Endpoints.GetTodoItems;
using TodoApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// add swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// register db context
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

// register endpoint handlers
services.AddEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// map endpoints
app.MapEndpoints();

app.Run();
