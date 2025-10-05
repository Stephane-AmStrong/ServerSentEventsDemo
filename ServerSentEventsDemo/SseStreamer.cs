using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace ServerSentEventsDemo;

public class SseStreamer(SseStreamingOptions options)
{
    public async Task StreamEventsAsync<TDto>(
        HttpContext ctx,
        IEventStreamingService<TDto> service,
        CancellationToken cancellationToken) where TDto : IBaseDto
    {
        SetResponseHeaders(ctx);

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

                var jsonEventData = JsonSerializer.Serialize(eventData, options.JsonOptions);

                await ctx.Response.WriteAsync(jsonEventData, cancellationToken);
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

    private static void SetResponseHeaders(HttpContext ctx)
    {
        ctx.Response.Headers.Append(HeaderNames.ContentType, "text/event-stream");
        ctx.Response.Headers.Append(HeaderNames.CacheControl, "no-cache");
        ctx.Response.Headers.Append("Connection", "keep-alive");
    }
}