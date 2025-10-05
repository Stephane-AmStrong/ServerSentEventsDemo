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
                SseStreamer streamer,
                CancellationToken cancellationToken) =>
            {
                await streamer.StreamEventsAsync(ctx, service, cancellationToken);
            })
            .WithName("ServerEvents")
            .WithSummary("Stream server events (created, updated, deleted)")
            .WithOpenApi();
        
        // SSE for clients
        sseGroup.MapGet("/clients", async (
                HttpContext ctx,
                IEventStreamingService<ClientResponse> service,
                SseStreamer streamer,
                CancellationToken cancellationToken) =>
            {
                await streamer.StreamEventsAsync(ctx, service, cancellationToken);
            })
            .WithName("ClientEvents")
            .WithSummary("Stream client events (created, updated, deleted)")
            .WithOpenApi();
    }
}