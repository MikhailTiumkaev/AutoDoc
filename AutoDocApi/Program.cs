using AutoDocApi.Endpoints;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using AutoDocApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebServices();
builder.Services.AddDataServices(builder.Configuration);

var app = builder.Build();

app.MapGroup("/todotasks").MapTodoTasksEndpoints();
app.MapGroup("/payloads").MapPayloadsEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await using var scope = app.Services.CreateAsyncScope();
    await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    //FOR DEMO PURPOSES ONLY
    dbContext.Database.Migrate();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();