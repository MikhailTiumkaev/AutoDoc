using AutoDocApi.Models;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using AutoDocApi.Contract;

namespace AutoDocApi.Endpoints
{
    public static class MapToDoTasksEndpoints
    {
        public static void MapTasksEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/todotasks", async (
                CreateTodoTaskRequest createTodoTaskRequest,
                AppDbContext dbContext,
                CancellationToken cancellationToken
            ) =>
            {
                TodoTask todoTask = new()
                {
                    Title = createTodoTaskRequest.Title,
                    Id = Guid.NewGuid(),
                    Status = "New",
                    DueDate = createTodoTaskRequest.DueDate
                };

                dbContext.TodoTasks.Add(todoTask);
                await dbContext.SaveChangesAsync(cancellationToken);

                return Results.Created($"/todotask/{todoTask!.Id}", todoTask);
            });

            app.MapGet("/todotasks", async (
                string status,
                int? page,
                int? pageSize,
                AppDbContext context,
                CancellationToken cancellationToken) =>
                {
                    var todoTask = await context.TodoTasks
                    .AsNoTracking()
                    .Where(t => t.Status == status)
                    .ToListAsync(cancellationToken: cancellationToken);

                    if (todoTask is null)
                    {
                        return Results.NoContent();
                    }

                    return Results.Ok(todoTask);
                });

            app.MapGet("/todotasks/{id}", async (
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
                    .FirstOrDefaultAsync(t => t.Id == guid, cancellationToken: cancellationToken);

                    if (todoTask is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(todoTask);
                });

            app.MapPut("/todotasks/{id}", async (string id, UpdateTodoTaskRequest dto, AppDbContext context) =>
            {
                if (!Guid.TryParse(id, out var guid))
                    {
                        return Results.BadRequest();
                    }

                var todoTask = await context.TodoTasks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == guid);

                if (todoTask is null) return Results.NotFound();

                todoTask.Title = dto.Title;
                todoTask.DueDate = dto.DueDate;
                todoTask.Status = dto.Status;

                await context.SaveChangesAsync();

                return Results.NoContent();
            });

            app.MapDelete("/todotasks/{id}", async (int id, AppDbContext context) =>
            {
                if (await context.TodoTasks.FindAsync(id) is TodoTask todoTask)
                {
                    context.TodoTasks.Remove(todoTask);
                    await context.SaveChangesAsync();
                    return Results.Ok();
                }

                return Results.NotFound();
            });
        }
    }
}