using AutoDocApi.Constants;
using AutoDocApi.Contract;
using AutoDocApi.Database;
using AutoDocApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoDocApi.Endpoints;

public static class TodoTasksHandler
{
    public static RouteGroupBuilder MapTodoTasksEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", CreateTodoTask);
        routes.MapGet("/", GetTodoTasksByQuery);
        routes.MapGet("/{id}", GetTodoTasksById);
        routes.MapPut("/{id}", UpdateTodoTask);
        routes.MapDelete("/{id}", DeleteTodoTask);
        routes.MapGet("/{id}/payloads", GetPayloadByTodoTaskId);
        return routes;
    }

    public static async Task<IResult> CreateTodoTask(
        CreateTodoTaskRequest createTodoTaskRequest,
        AppDbContext dbContext,
        CancellationToken ct = default)
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
    }

    public static async Task<IResult> GetTodoTasksByQuery(
        AppDbContext context,
        string? status = TaskStatusEnum.Pending,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
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
    }

        public static async Task<IResult> GetTodoTasksById(
        AppDbContext context,
        int id,
        CancellationToken ct = default)
    {
        var todoTask = await context.TodoTasks
                .AsNoTracking()
                .Include(t => t.Payloads)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken: ct);
            
        if (todoTask is null)
        {
            return Results.Problem(
                type: "Bad Request", 
                title: "TodoTask not found",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Results.Ok(todoTask);
    }

    public static async Task<IResult> UpdateTodoTask(
        AppDbContext context,
        int id,
        UpdateTodoTaskRequest dto,
        CancellationToken ct = default)
    {
        var todoTask = await context.TodoTasks
                .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (todoTask is null) return Results.NotFound();

        todoTask.Title = dto.Title;
        todoTask.DueDate = dto.DueDate;
        todoTask.Status = dto.Status;

        await context.SaveChangesAsync(ct);

        return Results.NoContent();
    }

    public static async Task<IResult> DeleteTodoTask(
        AppDbContext context,
        int id,
        CancellationToken ct = default)
    {
        if (await context.TodoTasks.FindAsync(id, ct) is TodoTask todoTask)
        {
            context.TodoTasks.Remove(todoTask);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        }

        return Results.NotFound();
    }

    public static async Task<IResult> GetPayloadByTodoTaskId(
        AppDbContext context,
        int id,
        int page = 1,
        int pageSize = 3,
        CancellationToken ct = default)
    {
        var tasksPayloads = await context.Payloads
                .AsNoTracking()
                .Where(t => t.TodoTaskId == id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: ct);

                return (tasksPayloads is null)
                    ? Results.NoContent()
                    : Results.Ok(tasksPayloads);
    }
}