using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace ServerSentEventsDemo;

public static class SseEndpoints
{
    public static void MapSseEndpoints(this IEndpointRouteBuilder app)
    {
        var sseGroup = app.MapGroup("/sse").WithTags("Server-Sent Events");

        // SSE pour les serveurs
        sseGroup.MapGet("/servers", async (
            HttpContext ctx,
            IEventStreamingService<ServerResponse> service,
            CancellationToken cancellationToken) =>
        {
            await StreamEventsAsync(ctx, service, cancellationToken);
        })
        .WithName("ServerEvents")
        .WithSummary("Stream server events (created, updated, deleted)")
        .WithOpenApi();
    }

    private static async Task StreamEventsAsync<TDto>(
        HttpContext ctx,
        IEventStreamingService<TDto> service,
        CancellationToken cancellationToken) where TDto : IBaseDto
    {
        ctx.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
        ctx.Response.Headers.Append(HeaderNames.CacheControl, "no-cache");
        ctx.Response.Headers.Append("Connection", "keep-alive");
        ctx.Response.Headers.Append("Access-Control-Allow-Origin", "*");

        try
        {
            while (await service.Events.WaitToReadAsync(cancellationToken))
            {
                var sseEvent = await service.Events.ReadAsync(cancellationToken);
                
                var eventData = new
                {
                    type = sseEvent.SseType.GetType().Name.Replace("Sse", "").Replace("Type", "").ToLowerInvariant(), // "created", "updated", "deleted"
                    data = sseEvent.Dto,
                    timestamp = DateTimeOffset.UtcNow
                };
                
                var json = JsonSerializer.Serialize(eventData, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });
                
                await ctx.Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await ctx.Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected - normal
        }
        finally
        {
            await ctx.Response.CompleteAsync();
        }
    }
}