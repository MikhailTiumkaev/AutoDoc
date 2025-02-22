using AutoDocApi.Models;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using AutoDocApi.Contract;

namespace AutoDocApi.Endpoints
{
    public static class MapEndpoints
    {
        public static void MapTasksEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/todotask", async (
                CreateTodoTaskRequest createTodoTaskRequest,
                AppDbContext dbContext,
                CancellationToken cancellationToken
            ) =>
            {
                TodoTask todoTask = new()
                {
                    Title = createTodoTaskRequest.Title,
                    DueDate = createTodoTaskRequest.DueDate
                };

                dbContext.TodoTasks.Add(todoTask);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Created($"/todotask/{todoTask!.Id}", todoTask);
            });

            app.MapGet("/todotask/{id}", async (
                string id,
                AppDbContext context,
                CancellationToken cancellationToken) =>
                {
                    if (!Guid.TryParse(id, out var guid))
                    {
                        return Results.BadRequest();
                    }

                    var todoTask = await context.TodoTasks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t=> t.Id == guid, cancellationToken: cancellationToken);

                    if (todoTask is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(todoTask);
                });

        }
    }
}