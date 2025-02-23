using AutoDocApi.Models;
using AutoDocApi.Database;
using Microsoft.EntityFrameworkCore;
using AutoDocApi.Contract;
using AutoDocApi.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AutoDocApi.Endpoints;

public static class TodoTasksEndpoints
{
    public static void MapTodoTasksEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/todotasks", async (
            CreateTodoTaskRequest createTodoTaskRequest,
            AppDbContext dbContext,
            CancellationToken ct
        ) =>
        {
            TodoTask todoTask = new()
            {
                Title = createTodoTaskRequest.Title,
                Status = TaskStatusEnum.Pending,
                DueDate = createTodoTaskRequest.DueDate
            };

            dbContext.TodoTasks.Add(todoTask);
            await dbContext.SaveChangesAsync(ct);

            return Results.Created($"/todotask/{todoTask!.Id}", todoTask);
        });

        app.MapGet("/todotasks", async (
                AppDbContext context,
                CancellationToken ct,
                string status,
                int page = 1,
                int pageSize = 10
            ) =>
            {
                var todoTask = await context.TodoTasks
                .AsNoTracking()
                .Where(t => t.Status == status)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: ct);

                if (todoTask is null)
                {
                    return Results.NoContent();
                }

                return Results.Ok(todoTask);
            });

        app.MapGet("/todotasks/{id}", async (
            int id,
            AppDbContext context,
            CancellationToken ct) =>
            {
                var todoTask = await context.TodoTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken: ct);

                if (todoTask is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(todoTask);
            });

        app.MapPut("/todotasks/{id}", async (
            int id,
            UpdateTodoTaskRequest dto,
            AppDbContext context,
            CancellationToken ct) =>
        {
            var todoTask = await context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == id, ct);

            if (todoTask is null) return Results.NotFound();

            todoTask.Title = dto.Title;
            todoTask.DueDate = dto.DueDate;
            todoTask.Status = dto.Status;

            await context.SaveChangesAsync(ct);

            return Results.NoContent();
        });

        app.MapDelete("/todotasks/{id}", async (int id, AppDbContext context) =>
        {
            if (await context.TodoTasks.FindAsync(id) is TodoTask todoTask)
            {
                context.TodoTasks.Remove(todoTask);
                await context.SaveChangesAsync();
                return Results.NoContent();
            }

            return Results.NotFound();
        });
    }
}
