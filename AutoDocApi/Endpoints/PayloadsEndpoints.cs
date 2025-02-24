using AutoDocApi.Database;
using AutoDocApi.FileServices;
using AutoDocApi.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace AutoDocApi.Endpoints;

public static class PayloadsHandler
{
    public static RouteGroupBuilder MapPayloadsEndpoints(this RouteGroupBuilder routes)
    {
        routes.MapPost("/{todoTaskId}/small/", UploadSmallFiles).DisableAntiforgery();
        routes.MapPost("/{todoTaskId}/big/", UploadBigFiles).DisableAntiforgery();
        routes.MapDelete("/{id}", DeletePayloadById);
        return routes;
    }

  public static async Task<IResult> UploadBigFiles(
        int todoTaskId,
        HttpRequest req,
        AppDbContext context,
        IFileService fileService,
        CancellationToken ct = default)
    {
        try
        {
            if (!MultipartRequestHelper.IsMultipartContentType(req.ContentType!))
            {
                throw new FormatException("Form without multipart content.");
            }
            var todoTask = await context.TodoTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == todoTaskId, cancellationToken: ct);

            if (todoTask is not null)
            {
                var payloads = new List<Payload>();

                // find the boundary
                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(req.ContentType));
                // use boundary to iterator through the multipart section
                var reader = new MultipartReader(boundary, req.Body);

                var section = await reader.ReadNextSectionAsync(ct);
                do
                {
                    var filePath = await fileService.SaveSectionAsFileAsync(section!, ct);
                    payloads.Add(new Payload
                    {
                        TodoTaskId = todoTaskId,
                        PayloadLocation = filePath
                    });

                    section = await reader.ReadNextSectionAsync(ct);
                } while (section != null);

                await context.Payloads.AddRangeAsync(payloads, ct);
                await context.SaveChangesAsync(ct);

                return Results.Ok(payloads);
            }
            return Results.NotFound();
        }          
        catch (Exception exception)
        {
            return Results.BadRequest($"Error: {exception.Message}");
        }
    }

   

    //works for small files, but for large files need to use MultipartReader
    public static async Task<IResult> UploadSmallFiles(
        int todoTaskId,
        IFormFileCollection files,
        AppDbContext context,
        IFileService fileService,
        CancellationToken ct = default)
    {
        var todoTask = await context.TodoTasks
                   .AsNoTracking()
                   .FirstOrDefaultAsync(t => t.Id == todoTaskId, cancellationToken: ct);

        if (todoTask is not null)
        {
            var payloads = new List<Payload>();
            foreach (var file in files)
            {
                var filePath = await fileService.SaveFormFileAsFileAsync(file!, ct);
                payloads.Add(new Payload
                {
                    TodoTaskId = todoTaskId,
                    PayloadLocation = filePath
                });
            }
            
            await context.Payloads.AddRangeAsync(payloads, ct);
            await context.SaveChangesAsync(ct);

            return Results.Ok();
        }

        return Results.NotFound();
    }

    public static async Task<IResult> DeletePayloadById(
        AppDbContext context,
        int id,
        CancellationToken ct = default)
    {
        if (await context.Payloads.FindAsync(id, ct) is Payload payload)
        {
            context.Payloads.Remove(payload);
            await context.SaveChangesAsync(ct);
            return Results.NoContent();
        }

        return Results.NotFound();
    }
}
