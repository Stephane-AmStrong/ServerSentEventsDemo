using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace ServerSentEventsDemo;

public static class SseEndpoints
{
    public static void MapSseEndpoints(this IEndpointRouteBuilder app)
    {
        var sseGroup = app.MapGroup("/sse").WithTags("Server-Sent Events");

        // SSE for servers
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
        
        // SSE for clients
        sseGroup.MapGet("/clients", async (
                HttpContext ctx,
                IEventStreamingService<ClientResponse> service,
                CancellationToken cancellationToken) =>
            {
                await StreamEventsAsync(ctx, service, cancellationToken);
            })
            .WithName("ClientEvents")
            .WithSummary("Stream client events (created, updated, deleted)")
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

        try
        {
            while (await service.Events.WaitToReadAsync(cancellationToken))
            {
                var broadcastMessage = await service.Events.ReadAsync(cancellationToken);

                var eventData = new
                {
                    type = broadcastMessage.MessageType.ToString().ToLowerInvariant(),
                    data = broadcastMessage.Dto,
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
        catch (Exception ex)
        {
            // Client disconnected - normal
        }
        finally
        {
            await ctx.Response.CompleteAsync();
        }
    }
}