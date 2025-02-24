using AutoDocApi.Constants;
using AutoDocApi.Database;
using AutoDocApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoDocApi.Configuration;
public static class DataConfiguration
{
    public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(
        options => options
        .UseNpgsql(configuration.GetConnectionString("Database"))

        //FOR DEMO PURPOSES ONLY     
        .UseSeeding(async (context, _) =>
        {
            if (context.Set<TodoTask>().Any()) return;

            var taskStatus = new[] { TaskStatusEnum.InProgress, TaskStatusEnum.Pending, TaskStatusEnum.Cancelled, TaskStatusEnum.Completed };
            var faker = new Bogus.Faker<TodoTask>()
                    .UseSeed(1337)
                    .RuleFor(x => x.Title, f => f.Lorem.Sentence())
                    .RuleFor(o => o.Status, f => f.PickRandom(taskStatus))
                    .RuleFor(x => x.DueDate, f => f.Date.Future().ToUniversalTime());

            var tasksToSeed = faker.Generate(1000);
            context.Set<TodoTask>().AddRange(tasksToSeed);
            await context.SaveChangesAsync();
        })
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            if (context.Set<TodoTask>().Any()) return;

            var taskStatus = new[] { TaskStatusEnum.InProgress, TaskStatusEnum.Pending, TaskStatusEnum.Cancelled, TaskStatusEnum.Completed };
            var faker = new Bogus.Faker<TodoTask>()
                    .UseSeed(1337)
                    .RuleFor(x => x.Title, f => f.Lorem.Sentence())
                    .RuleFor(o => o.Status, f => f.PickRandom(taskStatus))
                    .RuleFor(x => x.DueDate, f => f.Date.Future().ToUniversalTime());

            var tasksToSeed = faker.Generate(1000);
            context.Set<TodoTask>().AddRange(tasksToSeed);
            await context.SaveChangesAsync(ct);
        }));        
    }
}


