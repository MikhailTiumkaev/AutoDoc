using AutoDocApi.Database;
using AutoDocApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoDocApi.Endpoints;

public static class PayloadsHandler
{
    public static RouteGroupBuilder MapPayloadsEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/", UploadFiles).DisableAntiforgery();
        routes.MapGet("/{id}", DeletePayloadById);
        return routes;
    }

    public static async Task<IResult> UploadFiles(
        int todoTaskId,
        IFormFileCollection files,
        AppDbContext context,
        CancellationToken ct = default)
    {
        var todoTask = await context.TodoTasks
                   .AsNoTracking()
                   .FirstOrDefaultAsync(t => t.Id == todoTaskId, cancellationToken: ct);

        if (todoTask is not null)
        {
            foreach (var file in files)
            {
                using (var target = new MemoryStream())
                {
                    await file.CopyToAsync(target, ct);
                    context.Payloads.Add(new Payload
                    {
                        TodoTaskId = todoTaskId,
                        Content = target.ToArray()
                    });
                };

                await context.SaveChangesAsync(ct);
            }

            return Results.Ok();
        }

        return Results.NotFound();
    }

    public static async Task<IResult> DeletePayloadById(
        AppDbContext context,
        int id,
        CancellationToken ct = default)
    {
        if (await context.Payloads.FindAsync(id) is Payload payload)
            {
                context.Payloads.Remove(payload);
                await context.SaveChangesAsync();
                return Results.NoContent();
            }

        return Results.NotFound();
    }
}
