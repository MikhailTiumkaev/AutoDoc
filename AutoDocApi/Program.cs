using AutoDocApi.Endpoints;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using AutoDocApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024; // 1 GB
});

builder.Services.AddWebServices();
builder.Services.AddFileServices();
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
    await dbContext.Database.MigrateAsync();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();