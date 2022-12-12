var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/todoitems", () => "Hello World!");

app.MapGet("/todoitems/{id}", (int id) => "Hello World!");

app.MapPost("/todoitems", () =>
{
    return Results.Ok();
});

app.MapPut("/todoitems/{id}", (int id) =>
{
    return Results.Ok(id); });

app.MapDelete("/todoitems/{id}", (int id) =>
{
    return Results.NoContent();
});

app.Run();
