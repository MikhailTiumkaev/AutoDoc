using AutoDocApi.Endpoints;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using AutoDocApi.Models;
using AutoDocApi.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    options => options
        .UseNpgsql(builder.Configuration.GetConnectionString("Database"))
        //FOR DEMO PURPOSES ONLY
        .UseAsyncSeeding(async (context, _, ct) => {
            
            if(context.Set<TodoTask>().Any()) return;

            var taskStatus = new[] { TaskStatusEnum.InProgress, TaskStatusEnum.Pending, TaskStatusEnum.Cancelled, TaskStatusEnum.Done };
            var faker = new Bogus.Faker<TodoTask>()
                .UseSeed(1337)
                .RuleFor(x => x.Title, f => f.Lorem.Sentence())
                .RuleFor(o => o.Status, f => f.PickRandom(taskStatus))
                .RuleFor(x => x.DueDate, f => f.Date.Future().ToUniversalTime());

            var tasksToSeed =  faker.Generate(1000);
            context.Set<TodoTask>().AddRange(tasksToSeed);
            await context.SaveChangesAsync(ct);
        })
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await using var scope = app.Services.CreateAsyncScope();
    await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    await dbContext.Database.MigrateAsync();
    await dbContext.Database.EnsureCreatedAsync();

}

app.MapTasksEndpoints();

app.Run();