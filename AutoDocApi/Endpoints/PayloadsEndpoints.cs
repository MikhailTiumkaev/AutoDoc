using AutoDocApi.Database;
using AutoDocApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoDocApi.Endpoints;

public static class PayloadsEndpoints
{
    public static void MapPayloadsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/todotasks/{id}/payloads", async (
            int id,
            IFormFileCollection files,
            AppDbContext context,
            CancellationToken ct) =>
        {
            var todoTask = await context.TodoTasks
                   .AsNoTracking()
                   .FirstOrDefaultAsync(t => t.Id == id, cancellationToken: ct);

            if (todoTask is not null)
            {
                foreach (var file in files)
                {
                    using (var target = new MemoryStream())
                    {
                        await file.CopyToAsync(target, ct);
                        context.Payloads.Add(new Payload
                        {
                            TodoTaskId = id,
                            Content = target.ToArray()
                        });
                    };

                    await context.SaveChangesAsync(ct);
                }

                return Results.Ok();
            }

            return Results.NotFound();
        })
        .DisableAntiforgery();

        app.MapGet("/todotasks/{id}/payloads", async (
                AppDbContext context,
                CancellationToken ct,
                int id,
                int page = 1,
                int pageSize = 3
            ) =>
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

            });

        app.MapDelete("/payloads/{id}", async (int id, AppDbContext context) =>
        {
            if (await context.Payloads.FindAsync(id) is Payload payload)
            {
                context.Payloads.Remove(payload);
                await context.SaveChangesAsync();
                return Results.NoContent();
            }

            return Results.NotFound();
        });
    }
}
