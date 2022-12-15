using Microsoft.EntityFrameworkCore;
using TodoApi.Features.TodoItems.CreateTodoItem;
using TodoApi.Features.TodoItems.DeleteTodoItem;
using TodoApi.Features.TodoItems.GetTodoItem;
using TodoApi.Features.TodoItems.GetTodoItems;
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
    options.UseSqlServer(configuration.GetConnectionString("TodoItemsDbConnection"),
        sqlServerOptions =>
        {
            sqlServerOptions.MigrationsAssembly(typeof(TodoItemsDbContext).Assembly.FullName);
            // sqlServerOptions.MigrationsHistoryTable(
            //     TelematicsServiceDbContext.MigrationsHistoryTable,
            //     TelematicsServiceDbContext.DbSchema);
        });
});

// register handlers
services.AddScoped<CreateTodoItemEndpoint>();
services.AddScoped<GetTodoItemEndpoint>();
services.AddScoped<GetTodoItemsEndpoint>();
services.AddScoped<DeleteTodoItemEndpoint>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// map ednpoints

var scope = app.Services.CreateScope();

app.MapPost("/todoitems", (CreateTodoItemRequest request) => 
    scope.ServiceProvider.GetService<CreateTodoItemEndpoint>()?.HandleAsync(request));

app.MapGet("/todoitems", () => 
    scope.ServiceProvider.GetService<GetTodoItemsEndpoint>()?.HandleAsync());

app.MapGet("/todoitems/{id}", (int id) => 
    scope.ServiceProvider.GetService<GetTodoItemEndpoint>()?.HandleAsync(id));

app.MapDelete("/todoitems/{id}", (int id) => 
    scope.ServiceProvider.GetService<DeleteTodoItemEndpoint>()?.HandleAsync(id));



app.Run();
